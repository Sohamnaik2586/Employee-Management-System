using EmployeeManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        void Add(Employee employee);
        List<Employee> GetAll();
        Employee GetById(int id);

        void Update(Employee employee);

        void Delete(int id);
    }
}
