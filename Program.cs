using EmployeeManagement.Interfaces;
using EmployeeManagement.Repositories;
using EmployeeManagement.Services;
using EmployeeManagement.UI;

IEmployeeRepository repository = new InMemoryEmployeeRepository();
var employeeService = new EmployeeService(repository);
var application = new ConsoleApplication(employeeService);

application.Run();
