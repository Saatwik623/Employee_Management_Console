using EmployeeManagement.Models;

namespace EmployeeManagement.Api.Dtos;

/// <summary>Employee data returned by the API.</summary>
public sealed record EmployeeResponse(
    int Id,
    string Name,
    string Email,
    string Department,
    string EmploymentType,
    decimal MonthlyPay)
{
    public static EmployeeResponse FromEmployee(Employee employee) => new(
        employee.Id,
        employee.Name,
        employee.Email,
        employee.Department,
        employee.EmploymentType,
        employee.CalculateMonthlyPay());
}

/// <summary>Payload for creating a permanent employee.</summary>
public sealed record CreatePermanentEmployeeRequest(
    int Id,
    string Name,
    string Email,
    string Department,
    decimal MonthlySalary);

/// <summary>Payload for creating a contract employee.</summary>
public sealed record CreateContractEmployeeRequest(
    int Id,
    string Name,
    string Email,
    string Department,
    decimal HourlyRate,
    decimal HoursWorked);

/// <summary>Payload for updating an employee's basic information.</summary>
public sealed record UpdateEmployeeRequest(
    string Name,
    string Email,
    string Department);

/// <summary>Standard error payload returned for failed requests.</summary>
public sealed record ErrorResponse(string Message);
