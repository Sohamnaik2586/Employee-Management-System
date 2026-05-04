using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Common;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeesController(EmployeeService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create(CreateEmployeeDto dto)
        {
            _service.CreateEmployee(dto);
            return Ok(new ApiResponse<string>(true, "Employee created"));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var employees = _service.GetEmployees();
            return Ok(new ApiResponse<object>(true, "Employees fetched", employees));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employee = _service.GetEmployeeById(id);

            if (employee == null)
                return NotFound(new ApiResponse<string>(false, "Employee not found"));

            return Ok(new ApiResponse<object>(true, "Employee fetched", employee));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CreateEmployeeDto dto)
        {
            _service.UpdateEmployee(id, dto);
            return Ok(new ApiResponse<string>(true, "Employee updated"));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.DeleteEmployee(id);
            return Ok(new ApiResponse<string>(true, "Employee deleted"));
        }
    }
}