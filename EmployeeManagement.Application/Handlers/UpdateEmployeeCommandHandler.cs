using MediatR;
using EmployeeManagement.Application.Commands;
using EmployeeManagement.Application.Interfaces;

namespace EmployeeManagement.Application.Handlers
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
    {
        private readonly IEmployeeRepository _repository;

        public UpdateEmployeeCommandHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _repository.GetById(request.Id);

            if (employee is null)
                throw new Exception("Employee not found");

            employee.SetName(request.Name);
            employee.SetDepartment(request.Department);
            employee.SetSalary(request.Salary);

            _repository.Update(employee);
            return await Task.FromResult(Unit.Value);
        }
    }
}
