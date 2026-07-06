# Employee Management Console Application

Application status: Feature implementation completed.

A .NET 8 console application demonstrating:

- CRUD operations using `List<Employee>` (the update operation edits name, email, and department)
- LINQ filtering, searching, duplicate checks, and sorting
- Encapsulation, inheritance, abstraction, and polymorphism
- Graceful exception handling with custom exceptions
- All five SOLID principles
- Git feature-branch, conflict-resolution, and Pull Request workflow

## Run the application

```bash
dotnet restore
dotnet build
dotnet run
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
