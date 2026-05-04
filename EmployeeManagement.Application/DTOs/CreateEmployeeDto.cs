using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        [Range(0, 1000000000)]
        public decimal Salary { get; set; }
    }
}