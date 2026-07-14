namespace EmployeeManagement.Interfaces;

public interface IReadOnlyRepository<T>
{
    IReadOnlyCollection<T> GetAll();
    T? GetById(int id);
}
