using MediatR;
using EmployeeManagement.Application.Commands;
using EmployeeManagement.Application.Interfaces;

namespace EmployeeManagement.Application.Handlers
{
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IEmployeeRepository _repository;

        public DeleteEmployeeCommandHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _repository.GetById(request.Id);

            if (employee is null)
                throw new Exception("Employee not found");

            _repository.Delete(request.Id);
            return await Task.FromResult(Unit.Value);
        }
    }
}
