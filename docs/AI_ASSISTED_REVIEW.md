# AI-Assisted API Development & Review

**Tool used:** Claude Code (Anthropic), used in place of GitHub Copilot Chat named in
the original assignment brief. Same workflow: ask the assistant to design/generate the
API layer, review its output critically, and record what was kept vs. changed.

## What the AI generated

- The `EmployeeManagement.Api` project skeleton (`dotnet new webapi`), the
  `EmployeesController` with all seven CRUD/query endpoints, the request/response DTOs,
  the exception-handling middleware, and the DI wiring in `Program.cs`.
- The `EmployeeManagement.Tests` project and all 11 unit tests in `EmployeeServiceTests.cs`.
- The NSwag-generated client and the OpenAPI export command sequence.
- These two documentation files.

## Recommendations accepted

1. **Split the monolithic console project into a solution with a shared `EmployeeManagement.Core`
   library.** The original assignment kept `Models`/`Services`/`Repositories` inside the
   console project itself, which made them unreferenceable from a second project. Extracting
   them (namespaces unchanged) let the Web API, the test project, and the console app all
   depend on the exact same domain code — no duplication, no risk of the two apps' business
   rules drifting apart.

2. **Map domain exceptions to HTTP status codes in one place (`ExceptionHandlingMiddleware`)
   instead of try/catch in every controller action.** Keeps `EmployeesController` focused on
   the happy path; adding a new endpoint automatically gets correct error handling for free.

3. **Register the repository and service as `Singleton`, not `Scoped`.** Accepted because the
   backing store is `InMemoryEmployeeRepository` — a `List<Employee>` in memory. A `Scoped`
   (per-request) registration would silently reset the data on every single request, which
   would look like a working API that mysteriously "forgets" everything. This is called out
   explicitly because it's the kind of bug that only shows up when you make two requests in a
   row, not when you eyeball the code once.

4. **Pin `FluentAssertions` to `7.0.0` instead of the latest `8.x`.** FluentAssertions changed
   its license starting at v8: it now requires a paid commercial license above certain
   revenue thresholds and can prompt for license acceptance at runtime. `7.0.0` is the last
   Apache-2.0 release, which keeps the test project frictionless for a training project with
   no licensing ambiguity.

5. **Use C# `record` types for the DTOs** (`EmployeeResponse`, `CreatePermanentEmployeeRequest`,
   etc.) rather than classes with mutable properties. They're immutable, get structural
   equality for free (useful if tests ever compare DTOs directly), and the positional syntax
   keeps them to one line each.

## Recommendations reconsidered or rejected

1. **Rejected: generating the NSwag client via a live server call
   (`nswag openapi2csclient /input:http://localhost:5289/swagger/v1/swagger.json`).**
   That requires the API to be running at generation time, which is brittle for anyone
   re-running the generation later or in CI. Instead, the OpenAPI JSON is exported to a file
   first (`dotnet swagger tofile`) and NSwag reads that file — reproducible without a running
   server.

2. **Rejected: `XLOOKUP`-style newest C# 13/.NET 9 features, and `dotnet new webapi` defaults
   like minimal APIs.** The brief and the existing console project both target .NET 8 with
   controller-based MVC (`[ApiController]`), so the Web API follows the same style rather than
   mixing minimal-API endpoints with controllers in the same solution — consistency mattered
   more than using the newest syntax.

3. **Rejected: adding a real database (EF Core + SQL Server) for the Web API.** The assignment
   explicitly scopes this to the in-memory repository already built for the console app ("Test
   all APIs using Swagger UI" — no persistence requirement mentioned). Introducing EF Core here
   would be scope creep well beyond what was asked, and would also break the "same domain code
   powers both apps" property, since the console app has no database dependency today.

4. **Reconsidered: whether `Update` (PUT) should also allow changing salary/hourly-rate, not
   just name/email/department.** The underlying `EmployeeService.UpdateEmployee` (from the
   original assignment) only ever updates basic info — pay changes go through
   `ChangeMonthlySalary`/`ChangeContractTerms` on the model, which aren't exposed by
   `EmployeeService` at all. Rather than widening the API's contract beyond what the service
   layer supports (which would need new service methods not covered by this assignment), the
   `PUT` endpoint mirrors the existing service method exactly. Flagged here as a legitimate
   follow-up if a future task asks for it.

5. **Considered but left as-is: hardcoded `Swashbuckle.AspNetCore.Cli` and `NSwag.ConsoleCore`
   tool versions in `.config/dotnet-tools.json`.** The Swashbuckle CLI tool has to match the
   `Swashbuckle.AspNetCore` package version referenced by the API project (`6.6.2`) or it can
   fail to load the assembly via reflection — this was hit once during generation (the CLI's
   default `10.2.3` didn't match) and fixed by pinning explicitly. Left pinned rather than
   "latest" so `dotnet swagger tofile` keeps working without re-diagnosing the same mismatch.

## How to reproduce or extend this review

Re-run `dotnet build EmployeeManagement.sln` and `dotnet test EmployeeManagement.sln` after any
change — both are green as of this review. If you ask an AI assistant to extend the API (e.g.
add pagination, add authentication), have it state each design choice and the alternative it
rejected, the same way this document does, rather than silently picking one.
