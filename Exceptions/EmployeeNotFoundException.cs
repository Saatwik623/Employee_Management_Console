namespace EmployeeManagement.Exceptions;

public sealed class EmployeeNotFoundException : Exception
{
    public EmployeeNotFoundException(int employeeId)
        : base($"Employee with ID {employeeId} was not found.")
    {
    }
}
