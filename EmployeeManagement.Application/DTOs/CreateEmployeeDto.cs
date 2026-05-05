using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(25)]
        public string Department { get; set; }

        [Range(0, 10000000)]
        public decimal Salary { get; set; }
    }
}