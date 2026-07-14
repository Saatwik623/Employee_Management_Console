using System.Net.Mail;
using EmployeeManagement.Exceptions;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public sealed class EmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public void AddPermanentEmployee(
        int id,
        string name,
        string email,
        string department,
        decimal monthlySalary)
    {
        ValidateCommonFields(id, name, email, department);
        ValidatePositiveAmount(monthlySalary, nameof(monthlySalary));

        var employee = new PermanentEmployee(
            id,
            name,
            email,
            department,
            monthlySalary);

        _repository.Add(employee);
    }

    public void AddContractEmployee(
        int id,
        string name,
        string email,
        string department,
        decimal hourlyRate,
        decimal hoursWorked)
    {
        ValidateCommonFields(id, name, email, department);
        ValidatePositiveAmount(hourlyRate, nameof(hourlyRate));
        ValidateNonNegativeAmount(hoursWorked, nameof(hoursWorked));

        var employee = new ContractEmployee(
            id,
            name,
            email,
            department,
            hourlyRate,
            hoursWorked);

        _repository.Add(employee);
    }

    public IReadOnlyCollection<Employee> GetAllEmployees() => _repository.GetAll();

    public Employee GetEmployeeById(int id) =>
        _repository.GetById(id) ?? throw new EmployeeNotFoundException(id);

    public void UpdateEmployee(
        int id,
        string name,
        string email,
        string department)
    {
        ValidateCommonFields(id, name, email, department);

        Employee employee = GetEmployeeById(id);
        employee.UpdateBasicInformation(name, email, department);
        _repository.Update(employee);
    }

    public void DeleteEmployee(int id) => _repository.Delete(id);

    public IReadOnlyCollection<Employee> FindByDepartment(string department)
    {
        ValidateRequiredText(department, nameof(department));
        return _repository.FindByDepartment(department);
    }

    public IReadOnlyCollection<Employee> SortByMonthlyPayDescending() =>
        _repository.SortByMonthlyPayDescending();

    private static void ValidateCommonFields(
        int id,
        string name,
        string email,
        string department)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
        }

        ValidateRequiredText(name, nameof(name));
        ValidateRequiredText(email, nameof(email));
        ValidateRequiredText(department, nameof(department));

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("Enter a valid email address.", nameof(email), exception);
        }
    }

    private static void ValidateRequiredText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }
    }

    private static void ValidatePositiveAmount(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"{parameterName} must be greater than zero.");
        }
    }

    private static void ValidateNonNegativeAmount(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"{parameterName} cannot be negative.");
        }
    }
}
