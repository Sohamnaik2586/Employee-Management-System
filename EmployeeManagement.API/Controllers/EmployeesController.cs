using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Common;
using MediatR;
using EmployeeManagement.Application.Commands;
using EmployeeManagement.Application.Queries;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Creates a new employee record using the specified employee data.
        /// </summary>
        /// <param name="dto">The data transfer object containing the information required to create a new employee. Cannot be null.</param>
        /// <returns>An HTTP 200 OK response containing an ApiResponse with a success indicator and a confirmation message.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            var command = new CreateEmployeeCommand(dto.Name, dto.Department, dto.Salary);
            var employeeId = await _mediator.Send(command);
            return Ok(new ApiResponse<string>(true, "Employee created"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllEmployeesQuery();
            var employees = await _mediator.Send(query);
            return Ok(new ApiResponse<object>(true, "Employees fetched", employees));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEmployeeByIdQuery(id);
            var employee = await _mediator.Send(query);

            if (employee == null)
                return NotFound(new ApiResponse<string>(false, "Employee not found"));

            return Ok(new ApiResponse<object>(true, "Employee fetched", employee));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateEmployeeDto dto)
        {
            var command = new UpdateEmployeeCommand(id, dto.Name, dto.Department, dto.Salary);
            await _mediator.Send(command);
            return Ok(new ApiResponse<string>(true, "Employee updated"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteEmployeeCommand(id);
            await _mediator.Send(command);
            return Ok(new ApiResponse<string>(true, "Employee deleted"));
        }
    }
}