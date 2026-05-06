using MediatR;
using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Queries
{
    public class GetAllEmployeesQuery : IRequest<List<EmployeeDto>>
    {
    }
}
