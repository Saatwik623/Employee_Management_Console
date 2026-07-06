# Assignment Execution Guide

## 1. Prerequisites

Confirm the installed tools in the VS Code terminal:

```powershell
dotnet --version
git --version
```

The .NET command should show an `8.x.x` SDK. This assignment stores employees in an in-memory collection, so MySQL and SQL Server Management Studio are not required.

## 2. Create the initial project and main branch

```powershell
mkdir C:\Projects
cd C:\Projects
dotnet new console -n EmployeeManagement --framework net8.0
cd EmployeeManagement
code .
```

Create a file named `.gitignore`:

```text
bin/
obj/
.vs/
.vscode/
*.user
*.suo
```

Create a file named `README.md`:

```text
# Employee Management Console Application

Application status: Base version.
```

Initialize Git:

```powershell
git init
git branch -M main
git add .
git commit -m "chore: initialize .NET 8 console application"
```

## 3. Create the feature branch

```powershell
git switch -c feature/employee-management-console
```

Copy the completed source files from this reference project into the project folder. Then commit the implementation in logical groups.

```powershell
git add Models Interfaces Exceptions
git commit -m "feat: add employee domain models and repository contracts"

git add Repositories Services
git commit -m "feat: implement LINQ-based employee CRUD services"

git add UI Program.cs EmployeeManagement.csproj
git commit -m "feat: add interactive console menu and exception handling"

git add README.md ASSIGNMENT_GUIDE.md .gitignore
git commit -m "docs: document SOLID design and application workflow"
```

## 4. Build and run

```powershell
dotnet restore
dotnet build
dotnet run
```

## 5. Manual verification

Perform these tests:

1. Add a permanent employee.
2. Add a contract employee.
3. View all employees.
4. Search for an employee by ID.
5. Update the employee's name, email, and department.
6. Filter employees by department.
7. Sort employees by monthly pay.
8. Delete an employee.
9. Enter a duplicate ID and verify that a friendly error is displayed.
10. Enter an invalid email or numeric value and verify graceful exception handling.

## 6. Create and resolve a merge conflict

On the feature branch, change this README line:

```text
Application status: Base version.
```

to:

```text
Application status: Feature implementation completed.
```

Commit it:

```powershell
git add README.md
git commit -m "docs: mark feature implementation as completed"
```

Switch to `main` and edit the same README line differently:

```powershell
git switch main
```

Change the line to:

```text
Application status: Main branch documentation updated.
```

Commit it:

```powershell
git add README.md
git commit -m "docs: update application status on main"
```

Return to the feature branch and merge `main`:

```powershell
git switch feature/employee-management-console
git merge main
```

Git will report a conflict in `README.md`. VS Code will show conflict markers similar to:

```text
<<<<<<< HEAD
Application status: Feature implementation completed.
=======
Application status: Main branch documentation updated.
>>>>>>> main
```

Replace the entire conflict block with:

```text
Application status: Feature implementation completed and ready for review.
```

Then complete the conflict-resolution commit:

```powershell
git add README.md
git commit -m "merge: resolve README status conflict"
```

Verify the history:

```powershell
git log --oneline --graph --decorate --all
```

## 7. Push the repository

Create an empty repository on GitHub or GitLab. Do not initialize it with a README because the local repository already contains one.

Add the remote URL:

```powershell
git remote add origin <YOUR_REPOSITORY_URL>
```

Push both branches:

```powershell
git push -u origin main
git push -u origin feature/employee-management-console
```

## 8. Raise a Pull Request or Merge Request

Use these values:

- Base/target branch: `main`
- Compare/source branch: `feature/employee-management-console`
- Title: `Employee Management Console Application with SOLID Principles`

Suggested description:

```text
## Summary
- Added employee CRUD operations using an in-memory collection.
- Used LINQ for searching, duplicate checking, filtering, and sorting.
- Implemented permanent and contract employee types using OOP.
- Added input validation, custom exceptions, and graceful error handling.
- Refactored the application according to SRP, OCP, LSP, ISP, and DIP.
- Created and resolved a merge conflict as part of the Git workflow.

## Validation
- dotnet build completed successfully.
- CRUD operations were tested manually.
- Invalid input, duplicate IDs, and missing employee IDs were verified.
```

Add a reviewer and submit the Pull Request. GitLab uses the term **Merge Request**, but the branch workflow is the same.
