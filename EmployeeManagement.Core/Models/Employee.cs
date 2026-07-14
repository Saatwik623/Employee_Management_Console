namespace EmployeeManagement.Models;

public abstract class Employee
{
    protected Employee(int id, string name, string email, string department)
    {
        Id = id;
        UpdateBasicInformation(name, email, department);
    }

    public int Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;

    public void UpdateBasicInformation(string name, string email, string department)
    {
        Name = name.Trim();
        Email = email.Trim();
        Department = department.Trim();
    }

    public abstract string EmploymentType { get; }
    public abstract decimal CalculateMonthlyPay();

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Email: {Email}, Department: {Department}, " +
               $"Type: {EmploymentType}, Monthly Pay: {CalculateMonthlyPay():C}";
    }
}
