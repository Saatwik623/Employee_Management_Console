using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces;

public interface IEmployeeRepository :
    IReadOnlyRepository<Employee>,
    IWriteRepository<Employee>
{
    IReadOnlyCollection<Employee> FindByDepartment(string department);
    IReadOnlyCollection<Employee> SortByMonthlyPayDescending();
}
