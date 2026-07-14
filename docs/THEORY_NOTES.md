# Study Notes — ASP.NET Core & API Tooling

These notes cover the remaining theory topics from the internship plan. Each section
points to the actual code added to this repository so the concept isn't abstract.

## 1. Test-Driven Development (TDD)

TDD is a workflow, not a testing framework: you write a failing test first, write the
minimum code to pass it, then refactor while the test keeps you honest.

**Red → Green → Refactor**
1. **Red** — write a test for behavior that doesn't exist yet. It fails (often a compile
   error) because the production code isn't there.
2. **Green** — write the simplest code that makes the test pass. Resist adding anything
   the test doesn't require.
3. **Refactor** — clean up duplication or naming now that the test is a safety net. Re-run
   the test after every change; it must stay green.

**Why it helps:** tests written *before* the implementation describe behavior, not
implementation details, so they don't accidentally assert on internals. It also forces
every piece of logic to be reachable through a public API — code that's hard to test
is usually code with a design problem (a static call, a hidden dependency, mixed
concerns).

**Testable code checklist:**
- Depend on interfaces (`IEmployeeRepository`), not concrete classes — see
  [`Services/EmployeeService.cs`](../EmployeeManagement.Core/Services/EmployeeService.cs).
- Keep side effects (console I/O, HTTP, file access) at the edges of the app
  (`UI/ConsoleApplication.cs`, `Controllers/EmployeesController.cs`) and pure logic in
  the middle (`EmployeeService`, the `Employee` hierarchy).
- Throw specific exceptions (`EmployeeNotFoundException`, `DuplicateEmployeeException`)
  instead of returning magic values, so tests can assert on exact failure conditions.

The `EmployeeService` in this project was already structured this way from the console
assignment, which is exactly why it was easy to unit-test without any changes.

## 2. Unit Testing with xUnit & FluentAssertions

**xUnit** is the test runner/framework: it discovers test methods and reports
pass/fail. **FluentAssertions** is an assertion library that reads like a sentence and
gives much better failure messages than raw `Assert.Equal`.

Key xUnit attributes used in [`EmployeeManagement.Tests/EmployeeServiceTests.cs`](../EmployeeManagement.Tests/EmployeeServiceTests.cs):

| Attribute | Purpose |
|---|---|
| `[Fact]` | A single test case with no parameters. |
| `[Theory]` + `[InlineData(...)]` | The same test body run once per data row — used for the "salary must be positive" check with `0` and `-100`. |

**Arrange / Act / Assert** — every test in this project follows this shape:

```csharp
[Fact]
public void AddEmployee_WithDuplicateId_ThrowsDuplicateEmployeeException()
{
    // Arrange
    EmployeeService service = CreateService();
    service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);

    // Act
    Action act = () => service.AddPermanentEmployee(1, "Someone Else", "other@example.com", "Sales", 50000m);

    // Assert
    act.Should().Throw<DuplicateEmployeeException>();
}
```

FluentAssertions' `.Should()` extension works on almost any type: `result.Should().Be(x)`,
`collection.Should().HaveCount(2)`, `collection.Should().ContainInOrder(...)`. Note this
project pins `FluentAssertions` to **v7.0.0** — versions 8+ moved to a commercial license
for paid organizations, and 7.0.0 is the last Apache-2.0 release, which avoids any
license-acceptance step for a training exercise.

Run the suite with:

```powershell
dotnet test EmployeeManagement.Tests/EmployeeManagement.Tests.csproj
```

## 3. ASP.NET Core Fundamentals & Project Structure

An ASP.NET Core Web API is a console app under the hood — `Program.cs` is the entry
point — but the SDK (`Microsoft.NET.Sdk.Web`) adds a hosting layer.

**`Program.cs` — the three phases**, visible in
[`EmployeeManagement.Api/Program.cs`](../EmployeeManagement.Api/Program.cs):

1. **Build** — `WebApplication.CreateBuilder(args)` reads `appsettings.json`,
   environment variables, and command-line args into configuration.
2. **Register services (DI container)** — `builder.Services.Add...` calls register
   types so they can be constructor-injected later. This project registers:
   - `AddControllers()` — enables the MVC controller pipeline.
   - `AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>()` and
     `AddSingleton<EmployeeService>()` — one shared in-memory repository for the
     process lifetime, so data persists across requests (there's no database yet).
   - `AddSwaggerGen()` — generates the OpenAPI document.
3. **Configure the middleware pipeline** — `app.Use...` calls run in the order they're
   added, for every request. This project's pipeline is:
   `ExceptionHandlingMiddleware → Swagger (dev only) → HTTPS redirect → Authorization → MapControllers`.

**Dependency Injection** — `EmployeesController` never calls `new EmployeeService(...)`.
It declares the dependency in its constructor, and the container supplies it:

```csharp
public EmployeesController(EmployeeService employeeService) => _employeeService = employeeService;
```

This is the same Dependency Inversion Principle already applied in the console app —
ASP.NET Core just supplies the container that wires it up.

**Middleware** is a request/response pipeline where each component can inspect or short
-circuit a request. [`Middleware/ExceptionHandlingMiddleware.cs`](../EmployeeManagement.Api/Middleware/ExceptionHandlingMiddleware.cs)
wraps `_next(context)` in a try/catch so every controller's exceptions get converted to
a JSON error response with the right HTTP status code, instead of a raw stack trace.

## 4. REST API Design & HTTP Fundamentals

REST maps CRUD operations onto HTTP verbs against **resources** (nouns), not actions
(verbs) in the URL. The `EmployeesController` follows this:

| Verb | Route | Meaning | Status codes |
|---|---|---|---|
| `GET` | `/api/employees` | List all (optionally `?department=`) | 200 |
| `GET` | `/api/employees/sorted-by-pay` | List sorted by pay | 200 |
| `GET` | `/api/employees/{id}` | Get one | 200, 404 |
| `POST` | `/api/employees/permanent` | Create a permanent employee | 201, 400, 409 |
| `POST` | `/api/employees/contract` | Create a contract employee | 201, 400, 409 |
| `PUT` | `/api/employees/{id}` | Full update of name/email/department | 200, 400, 404 |
| `DELETE` | `/api/employees/{id}` | Remove | 204, 404 |

**Status code choices, and why:**
- **201 Created** on POST, with a `Location` header pointing at the new resource
  (`CreatedAtAction(nameof(GetById), ...)`), rather than 200 — the resource didn't
  exist before this call.
- **204 No Content** on DELETE — nothing meaningful to return.
- **404 Not Found** when `EmployeeNotFoundException` is thrown — the ID doesn't exist.
- **409 Conflict** when `DuplicateEmployeeException` is thrown — the request conflicts
  with existing state (same ID already used).
- **400 Bad Request** for any other `ArgumentException` (invalid email, negative salary,
  missing required field) — the request itself is malformed.

That mapping lives in `ExceptionHandlingMiddleware`, so controller methods stay focused
on the happy path and never write `if/else` status-code logic themselves.

**Model binding** — ASP.NET Core deserializes the JSON request body straight into the
DTOs in [`Dtos/EmployeeDtos.cs`](../EmployeeManagement.Api/Dtos/EmployeeDtos.cs)
(`CreatePermanentEmployeeRequest`, etc.) because `[ApiController]` enables automatic
`[FromBody]` inference for complex types on POST/PUT.

## 5. OpenAPI, Swagger & API Documentation

**OpenAPI** is a language-agnostic JSON/YAML specification describing an API's routes,
request/response shapes, and status codes. **Swagger** (via the `Swashbuckle.AspNetCore`
package) is the tool that *generates* that spec from the running ASP.NET Core app and
also serves an interactive UI to try requests.

What's wired up in this project:
- `AddSwaggerGen()` in `Program.cs` builds the spec from the controllers' routes,
  parameter types, and `[ProducesResponseType]` attributes.
- `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in
  [`EmployeeManagement.Api.csproj`](../EmployeeManagement.Api/EmployeeManagement.Api.csproj)
  makes the compiler emit an XML file of every `///` doc comment, which
  `options.IncludeXmlComments(xmlPath)` feeds into Swagger — that's why the descriptions
  on `GetAll`, `CreatePermanent`, etc. in the Swagger UI come straight from the C# doc
  comments on `EmployeesController`.
- `app.UseSwagger()` serves the raw JSON at `/swagger/v1/swagger.json`;
  `app.UseSwaggerUI()` serves the browsable page at `/swagger`.

A checked-in snapshot of the generated spec lives at
[`EmployeeManagement.Api/openapi.json`](../EmployeeManagement.Api/openapi.json) — it was
exported with the `dotnet swagger tofile` CLI (`Swashbuckle.AspNetCore.Cli`, added as a
local tool in `.config/dotnet-tools.json`) so a spec exists without needing the server
running.

## 6. NSwag & API Client Generation

**NSwag** consumes an OpenAPI document and generates a typed client (C#, TypeScript,
etc.) so consumers don't hand-write HTTP calls and JSON parsing.

This was done as a two-step, offline process (no server needed once the spec exists):

```powershell
# 1. Export the OpenAPI spec from the built API assembly
dotnet swagger tofile --output EmployeeManagement.Api/openapi.json EmployeeManagement.Api/bin/Debug/net8.0/EmployeeManagement.Api.dll v1

# 2. Generate a typed C# client from that spec
dotnet nswag openapi2csclient /input:EmployeeManagement.Api/openapi.json /classname:EmployeeManagementApiClient /namespace:EmployeeManagement.ApiClient /output:EmployeeManagement.ApiClient/EmployeeManagementApiClient.cs /GenerateClientInterfaces:true
```

The result is [`EmployeeManagement.ApiClient/EmployeeManagementApiClient.cs`](../EmployeeManagement.ApiClient/EmployeeManagementApiClient.cs)
— an auto-generated `EmployeeManagementApiClient` class implementing
`IEmployeeManagementApiClient`, with one strongly-typed async method per endpoint
(e.g. `EmployeesAllAsync(department)`, `PermanentAsync(request)`) and generated DTO
classes matching `EmployeeResponse` etc. It's packaged as its own class library so any
other .NET app in the solution (or outside it) can reference it and call the API without
writing `HttpClient` code by hand. Because the generated file is huge and
machine-written, it's never meant to be hand-edited — regenerate it whenever the API
contract changes.

**Contract-first vs. code-first:** this project used *code-first* (write the controllers,
generate the spec from them). The alternative, *contract-first*, is writing the OpenAPI
YAML by hand first and generating server stubs from it — useful when the spec is the
agreed contract between separate frontend/backend teams before any code exists.

## 7. AI-Assisted API Development

The assignment's original brief names GitHub Copilot Chat specifically; this project's
AI-assisted work was done with Claude (Claude Code) instead, performing the same role —
reviewing the generated API, suggesting fixes, and generating the unit tests and
documentation in this repo. See
[`docs/AI_ASSISTED_REVIEW.md`](AI_ASSISTED_REVIEW.md) for the concrete review notes,
and treat the workflow (ask the assistant to review a diff, evaluate each suggestion on
its merits, accept or reject explicitly) as the transferable skill regardless of which
AI tool a team standardizes on.
