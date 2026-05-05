using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public void CreateEmployee(CreateEmployeeDto dto)
        {
            var employee = new Employee(dto.Name, dto.Department, dto.Salary);
            _repository.Add(employee);
        }
        
        public List<EmployeeDto> GetEmployees()
        {
            
            return _repository.GetAll()
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department,
                    Salary = e.Salary
                })
                .ToList();
        }
        public EmployeeDto GetEmployeeById(int id)
        {
            var e = _repository.GetById(id);

            if (e == null) return null;

            return new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Department = e.Department,
                Salary = e.Salary
            };
        }

        public void UpdateEmployee(int id, CreateEmployeeDto dto)
        {
            var employee = _repository.GetById(id);

            if (employee == null)
                throw new Exception("Employee not found");

            employee.SetName(dto.Name);
            employee.SetDepartment(dto.Department);
            employee.SetSalary(dto.Salary);

            _repository.Update(employee);
        }

        public void DeleteEmployee(int id)
        {
            var employee = _repository.GetById(id);

            if (employee == null)
                throw new Exception("Employee not found");

            _repository.Delete(id);
        }
    }
}
