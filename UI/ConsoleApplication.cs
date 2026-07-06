using EmployeeManagement.Exceptions;
using EmployeeManagement.Models;
using EmployeeManagement.Services;

namespace EmployeeManagement.UI;

public sealed class ConsoleApplication
{
    private readonly EmployeeService _employeeService;

    public ConsoleApplication(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public void Run()
    {
        bool shouldExit = false;

        while (!shouldExit)
        {
            DisplayMenu();
            string choice = ReadRequiredText("Choose an option: ");
            Console.WriteLine();

            try
            {
                shouldExit = ExecuteChoice(choice);
            }
            catch (EmployeeNotFoundException exception)
            {
                WriteError(exception.Message);
            }
            catch (DuplicateEmployeeException exception)
            {
                WriteError(exception.Message);
            }
            catch (ArgumentException exception)
            {
                WriteError(exception.Message);
            }
            catch (Exception exception)
            {
                WriteError($"Unexpected error: {exception.Message}");
            }

            if (!shouldExit)
            {
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private bool ExecuteChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                AddEmployee();
                return false;
            case "2":
                DisplayEmployees(_employeeService.GetAllEmployees());
                return false;
            case "3":
                ViewEmployeeById();
                return false;
            case "4":
                UpdateEmployee();
                return false;
            case "5":
                DeleteEmployee();
                return false;
            case "6":
                FilterEmployeesByDepartment();
                return false;
            case "7":
                DisplayEmployees(_employeeService.SortByMonthlyPayDescending());
                return false;
            case "0":
                Console.WriteLine("Application closed.");
                return true;
            default:
                WriteError("Invalid option. Select a number from 0 to 7.");
                return false;
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("=======================================");
        Console.WriteLine("     EMPLOYEE MANAGEMENT SYSTEM");
        Console.WriteLine("=======================================");
        Console.WriteLine("1. Add employee");
        Console.WriteLine("2. View all employees");
        Console.WriteLine("3. View employee by ID");
        Console.WriteLine("4. Update employee");
        Console.WriteLine("5. Delete employee");
        Console.WriteLine("6. Filter employees by department");
        Console.WriteLine("7. Sort employees by monthly pay");
        Console.WriteLine("0. Exit");
        Console.WriteLine("=======================================");
    }

    private void AddEmployee()
    {
        Console.WriteLine("Employee type:");
        Console.WriteLine("1. Permanent");
        Console.WriteLine("2. Contract");

        string type = ReadRequiredText("Choose type: ");
        int id = ReadInt("Employee ID: ");
        string name = ReadRequiredText("Name: ");
        string email = ReadRequiredText("Email: ");
        string department = ReadRequiredText("Department: ");

        switch (type)
        {
            case "1":
                decimal monthlySalary = ReadDecimal("Monthly salary: ");
                _employeeService.AddPermanentEmployee(
                    id,
                    name,
                    email,
                    department,
                    monthlySalary);
                break;

            case "2":
                decimal hourlyRate = ReadDecimal("Hourly rate: ");
                decimal hoursWorked = ReadDecimal("Hours worked this month: ");
                _employeeService.AddContractEmployee(
                    id,
                    name,
                    email,
                    department,
                    hourlyRate,
                    hoursWorked);
                break;

            default:
                throw new ArgumentException("Invalid employee type.");
        }

        WriteSuccess("Employee added successfully.");
    }

    private void ViewEmployeeById()
    {
        int id = ReadInt("Enter employee ID: ");
        Employee employee = _employeeService.GetEmployeeById(id);
        Console.WriteLine(employee);
    }

    private void UpdateEmployee()
    {
        int id = ReadInt("Enter employee ID to update: ");
        Employee existingEmployee = _employeeService.GetEmployeeById(id);

        Console.WriteLine($"Editing: {existingEmployee.Name} ({existingEmployee.EmploymentType})");
        string name = ReadRequiredText("New name: ");
        string email = ReadRequiredText("New email: ");
        string department = ReadRequiredText("New department: ");

        _employeeService.UpdateEmployee(id, name, email, department);
        WriteSuccess("Employee updated successfully.");
    }

    private void DeleteEmployee()
    {
        int id = ReadInt("Enter employee ID to delete: ");
        Employee employee = _employeeService.GetEmployeeById(id);

        string confirmation = ReadRequiredText(
            $"Delete {employee.Name}? Type YES to confirm: ");

        if (!confirmation.Equals("YES", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Delete cancelled.");
            return;
        }

        _employeeService.DeleteEmployee(id);
        WriteSuccess("Employee deleted successfully.");
    }

    private void FilterEmployeesByDepartment()
    {
        string department = ReadRequiredText("Enter department: ");
        DisplayEmployees(_employeeService.FindByDepartment(department));
    }

    private static void DisplayEmployees(IEnumerable<Employee> employees)
    {
        List<Employee> employeeList = employees.ToList();

        if (employeeList.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        foreach (Employee employee in employeeList)
        {
            Console.WriteLine(employee);
        }
    }

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int value))
        {
            throw new ArgumentException("Enter a valid whole number.");
        }

        return value;
    }

    private static decimal ReadDecimal(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal value))
        {
            throw new ArgumentException("Enter a valid numeric amount.");
        }

        return value;
    }

    private static string ReadRequiredText(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("This value is required.");
        }

        return input;
    }

    private static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
