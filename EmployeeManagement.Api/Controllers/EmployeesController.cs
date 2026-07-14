using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

/// <summary>CRUD operations for employees, backed by the in-memory repository.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class EmployeesController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public EmployeesController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>Gets every employee, optionally filtered by department.</summary>
    /// <param name="department">Optional department name to filter by.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EmployeeResponse>> GetAll([FromQuery] string? department)
    {
        IReadOnlyCollection<Employee> employees = string.IsNullOrWhiteSpace(department)
            ? _employeeService.GetAllEmployees()
            : _employeeService.FindByDepartment(department);

        return Ok(employees.Select(EmployeeResponse.FromEmployee));
    }

    /// <summary>Gets all employees ordered by monthly pay, highest first.</summary>
    [HttpGet("sorted-by-pay")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EmployeeResponse>> GetSortedByPay()
    {
        IReadOnlyCollection<Employee> employees = _employeeService.SortByMonthlyPayDescending();
        return Ok(employees.Select(EmployeeResponse.FromEmployee));
    }

    /// <summary>Gets a single employee by ID.</summary>
    /// <param name="id">The employee ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<EmployeeResponse> GetById(int id)
    {
        Employee employee = _employeeService.GetEmployeeById(id);
        return Ok(EmployeeResponse.FromEmployee(employee));
    }

    /// <summary>Creates a permanent employee.</summary>
    [HttpPost("permanent")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<EmployeeResponse> CreatePermanent(CreatePermanentEmployeeRequest request)
    {
        _employeeService.AddPermanentEmployee(
            request.Id,
            request.Name,
            request.Email,
            request.Department,
            request.MonthlySalary);

        Employee employee = _employeeService.GetEmployeeById(request.Id);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, EmployeeResponse.FromEmployee(employee));
    }

    /// <summary>Creates a contract employee.</summary>
    [HttpPost("contract")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public ActionResult<EmployeeResponse> CreateContract(CreateContractEmployeeRequest request)
    {
        _employeeService.AddContractEmployee(
            request.Id,
            request.Name,
            request.Email,
            request.Department,
            request.HourlyRate,
            request.HoursWorked);

        Employee employee = _employeeService.GetEmployeeById(request.Id);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, EmployeeResponse.FromEmployee(employee));
    }

    /// <summary>Updates an employee's name, email, and department.</summary>
    /// <param name="id">The employee ID.</param>
    /// <param name="request">The updated name, email, and department.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<EmployeeResponse> Update(int id, UpdateEmployeeRequest request)
    {
        _employeeService.UpdateEmployee(id, request.Name, request.Email, request.Department);
        Employee employee = _employeeService.GetEmployeeById(id);
        return Ok(EmployeeResponse.FromEmployee(employee));
    }

    /// <summary>Deletes an employee by ID.</summary>
    /// <param name="id">The employee ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        _employeeService.DeleteEmployee(id);
        return NoContent();
    }
}
