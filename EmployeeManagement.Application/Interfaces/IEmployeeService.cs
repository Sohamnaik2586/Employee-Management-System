using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Interfaces
{
    /// <summary>
    /// Interface for Employee Service
    /// Allows for mocking and dependency injection
    /// </summary>
    public interface IEmployeeService
    {
        void CreateEmployee(CreateEmployeeDto dto);
        List<EmployeeDto> GetEmployees();
        EmployeeDto GetEmployeeById(int id);
        void UpdateEmployee(int id, CreateEmployeeDto dto);
        void DeleteEmployee(int id);
    }
}
