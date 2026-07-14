namespace EmployeeManagement.Exceptions;

public sealed class DuplicateEmployeeException : Exception
{
    public DuplicateEmployeeException(int employeeId)
        : base($"An employee with ID {employeeId} already exists.")
    {
    }
}
