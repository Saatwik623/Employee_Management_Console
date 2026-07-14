using EmployeeManagement.Exceptions;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories;

public sealed class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees = [];

    public void Add(Employee employee)
    {
        bool alreadyExists = _employees.Any(item => item.Id == employee.Id);
        if (alreadyExists)
        {
            throw new DuplicateEmployeeException(employee.Id);
        }

        _employees.Add(employee);
    }

    public IReadOnlyCollection<Employee> GetAll() => _employees.ToList().AsReadOnly();

    public Employee? GetById(int id) =>
        _employees.FirstOrDefault(employee => employee.Id == id);

    public void Update(Employee employee)
    {
        int index = _employees.FindIndex(item => item.Id == employee.Id);
        if (index < 0)
        {
            throw new EmployeeNotFoundException(employee.Id);
        }

        _employees[index] = employee;
    }

    public void Delete(int id)
    {
        Employee employee = GetById(id) ?? throw new EmployeeNotFoundException(id);
        _employees.Remove(employee);
    }

    public IReadOnlyCollection<Employee> FindByDepartment(string department) =>
        _employees
            .Where(employee => employee.Department.Equals(
                department,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(employee => employee.Name)
            .ToList()
            .AsReadOnly();

    public IReadOnlyCollection<Employee> SortByMonthlyPayDescending() =>
        _employees
            .OrderByDescending(employee => employee.CalculateMonthlyPay())
            .ThenBy(employee => employee.Name)
            .ToList()
            .AsReadOnly();
}
