# Employee Management

Application status: Console app, Web API, and unit tests completed and ready for review.

A .NET 8 solution with a domain layer shared between a console application and an
ASP.NET Core Web API:

- CRUD operations using `List<Employee>` (the update operation edits name, email, and department)
- LINQ filtering, searching, duplicate checks, and sorting
- Encapsulation, inheritance, abstraction, and polymorphism
- Graceful exception handling with custom exceptions
- All five SOLID principles
- Git feature-branch, conflict-resolution, and Pull Request workflow

## Projects

| Project | Purpose |
|---|---|
| `EmployeeManagement` | Console UI (original assignment). |
| `EmployeeManagement.Core` | Shared domain layer: `Models`, `Interfaces`, `Repositories`, `Services`, `Exceptions`. |
| `EmployeeManagement.Api` | ASP.NET Core Web API exposing the same domain layer over REST, with Swagger UI. |
| `EmployeeManagement.Tests` | xUnit + FluentAssertions unit tests for `EmployeeService`. |
| `EmployeeManagement.ApiClient` | NSwag-generated typed C# client for the Web API. |

See [`docs/THEORY_NOTES.md`](docs/THEORY_NOTES.md) for the concepts behind each project
and [`docs/AI_ASSISTED_REVIEW.md`](docs/AI_ASSISTED_REVIEW.md) for the AI-assisted design
review of the API layer.

## Run the console app

```bash
dotnet restore
dotnet build
dotnet run --project EmployeeManagement.csproj
```

## Run the Web API

```bash
dotnet run --project EmployeeManagement.Api/EmployeeManagement.Api.csproj
```

Then open `https://localhost:<port>/swagger` for the interactive Swagger UI.

## Run the tests

```bash
dotnet test EmployeeManagement.Tests/EmployeeManagement.Tests.csproj
```

## SOLID mapping

### SRP — Single Responsibility Principle

- `Employee` classes represent employee data and pay behavior.
- `InMemoryEmployeeRepository` manages collection storage.
- `EmployeeService` performs validation and use-case logic.
- `ConsoleApplication` handles user input and output.
- `Program.cs` only composes dependencies.

### OCP — Open/Closed Principle

`Employee.CalculateMonthlyPay()` is polymorphic. A new employee subtype can define its own pay calculation without changing repository sorting or reporting logic.

### LSP — Liskov Substitution Principle

`PermanentEmployee` and `ContractEmployee` can both be used wherever the base `Employee` type is expected.

### ISP — Interface Segregation Principle

Repository contracts are divided into `IReadOnlyRepository<T>` and `IWriteRepository<T>`. `IEmployeeRepository` composes them and adds employee-specific queries.

### DIP — Dependency Inversion Principle

`EmployeeService` depends on the `IEmployeeRepository` abstraction rather than directly depending on `InMemoryEmployeeRepository`.

## Example test data

Permanent employee:

- ID: 101
- Name: Asha Sharma
- Email: asha@example.com
- Department: Engineering
- Monthly salary: 85000

Contract employee:

- ID: 102
- Name: Rahul Verma
- Email: rahul@example.com
- Department: Quality Assurance
- Hourly rate: 900
- Hours worked: 160
