namespace EmployeeManagement.Interfaces;

public interface IWriteRepository<T>
{
    void Add(T item);
    void Update(T item);
    void Delete(int id);
}
