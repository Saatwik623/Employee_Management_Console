using EmployeeManagement.Exceptions;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Tests;

public sealed class EmployeeServiceTests
{
    private static EmployeeService CreateService()
    {
        IEmployeeRepository repository = new InMemoryEmployeeRepository();
        return new EmployeeService(repository);
    }

    [Fact]
    public void AddPermanentEmployee_WithValidData_IsRetrievableAfterwards()
    {
        EmployeeService service = CreateService();

        service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);

        Employee employee = service.GetEmployeeById(1);
        employee.Should().BeOfType<PermanentEmployee>();
        employee.Name.Should().Be("Asha Sharma");
        employee.CalculateMonthlyPay().Should().Be(85000m);
    }

    [Fact]
    public void AddContractEmployee_CalculatesMonthlyPayFromRateAndHours()
    {
        EmployeeService service = CreateService();

        service.AddContractEmployee(2, "Rahul Verma", "rahul@example.com", "QA", 900m, 160m);

        Employee employee = service.GetEmployeeById(2);
        employee.CalculateMonthlyPay().Should().Be(900m * 160m);
    }

    [Fact]
    public void AddEmployee_WithDuplicateId_ThrowsDuplicateEmployeeException()
    {
        EmployeeService service = CreateService();
        service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);

        Action act = () => service.AddPermanentEmployee(1, "Someone Else", "other@example.com", "Sales", 50000m);

        act.Should().Throw<DuplicateEmployeeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void AddPermanentEmployee_WithNonPositiveSalary_ThrowsArgumentOutOfRangeException(decimal salary)
    {
        EmployeeService service = CreateService();

        Action act = () => service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", salary);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddPermanentEmployee_WithInvalidEmail_ThrowsArgumentException()
    {
        EmployeeService service = CreateService();

        Action act = () => service.AddPermanentEmployee(1, "Asha Sharma", "not-an-email", "Engineering", 85000m);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetEmployeeById_WhenMissing_ThrowsEmployeeNotFoundException()
    {
        EmployeeService service = CreateService();

        Action act = () => service.GetEmployeeById(999);

        act.Should().Throw<EmployeeNotFoundException>();
    }

    [Fact]
    public void UpdateEmployee_ChangesBasicInformation()
    {
        EmployeeService service = CreateService();
        service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);

        service.UpdateEmployee(1, "Asha Verma", "asha.verma@example.com", "Platform");

        Employee employee = service.GetEmployeeById(1);
        employee.Name.Should().Be("Asha Verma");
        employee.Email.Should().Be("asha.verma@example.com");
        employee.Department.Should().Be("Platform");
    }

    [Fact]
    public void DeleteEmployee_RemovesEmployeeFromRepository()
    {
        EmployeeService service = CreateService();
        service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);

        service.DeleteEmployee(1);

        Action act = () => service.GetEmployeeById(1);
        act.Should().Throw<EmployeeNotFoundException>();
    }

    [Fact]
    public void FindByDepartment_ReturnsOnlyMatchingEmployeesOrderedByName()
    {
        EmployeeService service = CreateService();
        service.AddPermanentEmployee(1, "Zara Khan", "zara@example.com", "Engineering", 90000m);
        service.AddPermanentEmployee(2, "Asha Sharma", "asha@example.com", "Engineering", 85000m);
        service.AddContractEmployee(3, "Rahul Verma", "rahul@example.com", "QA", 900m, 160m);

        IReadOnlyCollection<Employee> engineering = service.FindByDepartment("engineering");

        engineering.Should().HaveCount(2);
        engineering.Select(e => e.Name).Should().ContainInOrder("Asha Sharma", "Zara Khan");
    }

    [Fact]
    public void SortByMonthlyPayDescending_OrdersHighestPayFirst()
    {
        EmployeeService service = CreateService();
        service.AddPermanentEmployee(1, "Asha Sharma", "asha@example.com", "Engineering", 85000m);
        service.AddContractEmployee(2, "Rahul Verma", "rahul@example.com", "QA", 900m, 160m); // 144000
        service.AddPermanentEmployee(3, "Zara Khan", "zara@example.com", "Sales", 60000m);

        IReadOnlyCollection<Employee> sorted = service.SortByMonthlyPayDescending();

        sorted.Select(e => e.Id).Should().ContainInOrder(2, 1, 3);
    }
}
