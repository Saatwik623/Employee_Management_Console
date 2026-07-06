namespace EmployeeManagement.Models;

public sealed class PermanentEmployee : Employee
{
    public PermanentEmployee(
        int id,
        string name,
        string email,
        string department,
        decimal monthlySalary)
        : base(id, name, email, department)
    {
        ChangeMonthlySalary(monthlySalary);
    }

    public decimal MonthlySalary { get; private set; }
    public override string EmploymentType => "Permanent";

    public void ChangeMonthlySalary(decimal monthlySalary)
    {
        if (monthlySalary <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(monthlySalary),
                "Monthly salary must be greater than zero.");
        }

        MonthlySalary = monthlySalary;
    }

    public override decimal CalculateMonthlyPay() => MonthlySalary;
}
