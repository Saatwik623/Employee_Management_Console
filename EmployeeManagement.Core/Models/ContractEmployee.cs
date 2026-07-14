namespace EmployeeManagement.Models;

public sealed class ContractEmployee : Employee
{
    public ContractEmployee(
        int id,
        string name,
        string email,
        string department,
        decimal hourlyRate,
        decimal hoursWorked)
        : base(id, name, email, department)
    {
        ChangeContractTerms(hourlyRate, hoursWorked);
    }

    public decimal HourlyRate { get; private set; }
    public decimal HoursWorked { get; private set; }
    public override string EmploymentType => "Contract";

    public void ChangeContractTerms(decimal hourlyRate, decimal hoursWorked)
    {
        if (hourlyRate <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hourlyRate),
                "Hourly rate must be greater than zero.");
        }

        if (hoursWorked < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hoursWorked),
                "Hours worked cannot be negative.");
        }

        HourlyRate = hourlyRate;
        HoursWorked = hoursWorked;
    }

    public override decimal CalculateMonthlyPay() => HourlyRate * HoursWorked;
}
