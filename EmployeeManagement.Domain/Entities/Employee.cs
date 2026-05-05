using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public int Id { get; private set; }
        public string Name { get; private set; }   
        public String Department { get; private set; }
        public decimal Salary { get; private set; }
        public Employee(string name, string department, decimal salary)
        {
            SetName(name);
            SetDepartment(department);
            SetSalary(salary);
        }
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.");
            }
            if(name.Length > 50)
            {
                throw new ArgumentException("Name cannot exceed 50 characters.");
            }
            Name = name;
        }
        public void SetDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
            {
                throw new ArgumentException("Department cannot be empty.");
            }
            if(department.Length > 25)
            {
                throw new ArgumentException("Department cannot exceed 25 characters.");
            }
            Department = department;
        }
        public void SetSalary(decimal salary)
        {
            if (salary < 0)
            {
                throw new ArgumentException("Salary cannot be negative.");
            }
            Salary = salary;
        }
    }
}
