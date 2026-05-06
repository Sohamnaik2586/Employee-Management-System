using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.API.Controllers;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Common;
using MediatR;
using EmployeeManagement.Application.Commands;
using EmployeeManagement.Application.Queries;

namespace EmployeeManagement.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the EmployeesController.
    /// Tests API endpoint behavior and HTTP response handling.
    /// Uses IMediator interface for mocking, which is testable with Moq.
    /// </summary>
    public class EmployeesControllerTests
    {
        private EmployeesController CreateController(Mock<IMediator> mockMediator = null)
        {
            mockMediator ??= new Mock<IMediator>();
            return new EmployeesController(mockMediator.Object);
        }

        // ========================================================================
        // SECTION 1: CREATE ENDPOINT TESTS - POST /api/employees
        // ========================================================================

        [Fact]
        public async Task Create_WithValidDto_Returns200OkResult()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // ACT
            var result = await controller.Create(createDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee created", response.Message);
        }

        [Fact]
        public async Task Create_CallsMediatorSendExactlyOnce()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // ACT
            await controller.Create(createDto);

            // ASSERT
            mockMediator.Verify(
                m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()), 
                Times.Once, 
                "Mediator.Send should be called exactly once"
            );
        }

        [Fact]
        public async Task Create_PassesCorrectCommandToMediator()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var createDto = new CreateEmployeeDto
            {
                Name = "Jane Smith",
                Department = "HR",
                Salary = 60000
            };

            CreateEmployeeCommand capturedCommand = null;
            mockMediator
                .Setup(m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<int>, CancellationToken>((cmd, ct) => capturedCommand = cmd as CreateEmployeeCommand)
                .ReturnsAsync(1);

            // ACT
            await controller.Create(createDto);

            // ASSERT
            Assert.NotNull(capturedCommand);
            Assert.Equal("Jane Smith", capturedCommand.Name);
            Assert.Equal("HR", capturedCommand.Department);
            Assert.Equal(60000, capturedCommand.Salary);
        }

        // ========================================================================
        // SECTION 2: GET ALL ENDPOINT TESTS - GET /api/employees
        // ========================================================================

        [Fact]
        public async Task GetAll_WithEmployeesInDatabase_Returns200OkWithData()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = 1, Name = "John Doe", Department = "IT", Salary = 50000 },
                new EmployeeDto { Id = 2, Name = "Jane Smith", Department = "HR", Salary = 60000 }
            };
            mockMediator
                .Setup(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // ACT
            var result = await controller.GetAll();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employees fetched", response.Message);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task GetAll_WithNoEmployees_Returns200OkWithEmptyList()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeDto>());

            // ACT
            var result = await controller.GetAll();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetAll_CallsMediatorSendExactlyOnce()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeDto>());

            // ACT
            await controller.GetAll();

            // ASSERT
            mockMediator.Verify(
                m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()), 
                Times.Once, 
                "Mediator.Send should be called exactly once"
            );
        }

        // ========================================================================
        // SECTION 3: GET BY ID ENDPOINT TESTS - GET /api/employees/{id}
        // ========================================================================

        [Fact]
        public async Task GetById_WithValidId_Returns200OkWithEmployee()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetEmployeeByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDto);

            // ACT
            var result = await controller.GetById(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee fetched", response.Message);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetEmployeeByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Employee not found"));

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => controller.GetById(999));
        }

        [Fact]
        public async Task GetById_PassesCorrectQueryToMediator()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var employeeDto = new EmployeeDto
            {
                Id = 5,
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            GetEmployeeByIdQuery capturedQuery = null;
            mockMediator
                .Setup(m => m.Send(It.IsAny<GetEmployeeByIdQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<EmployeeDto>, CancellationToken>((query, ct) => capturedQuery = query as GetEmployeeByIdQuery)
                .ReturnsAsync(employeeDto);

            // ACT
            await controller.GetById(5);

            // ASSERT
            Assert.NotNull(capturedQuery);
            Assert.Equal(5, capturedQuery.Id);
        }

        // ========================================================================
        // SECTION 4: UPDATE ENDPOINT TESTS - PUT /api/employees/{id}
        // ========================================================================

        [Fact]
        public async Task Update_WithValidData_Returns200Ok()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane Doe",
                Department = "HR",
                Salary = 60000
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // ACT
            var result = await controller.Update(1, updateDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee updated", response.Message);
        }

        [Fact]
        public async Task Update_PassesCorrectCommandToMediator()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane Doe",
                Department = "HR",
                Salary = 60000
            };

            UpdateEmployeeCommand capturedCommand = null;
            mockMediator
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Unit>, CancellationToken>((cmd, ct) => capturedCommand = cmd as UpdateEmployeeCommand)
                .ReturnsAsync(Unit.Value);

            // ACT
            await controller.Update(1, updateDto);

            // ASSERT
            Assert.NotNull(capturedCommand);
            Assert.Equal(1, capturedCommand.Id);
            Assert.Equal("Jane Doe", capturedCommand.Name);
            Assert.Equal("HR", capturedCommand.Department);
            Assert.Equal(60000, capturedCommand.Salary);
        }

        [Fact]
        public async Task Update_WhenMediatorThrowsException_PropagatesException()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane",
                Department = "HR",
                Salary = 60000
            };

            mockMediator
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Employee not found"));

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => controller.Update(999, updateDto));
        }

        // ========================================================================
        // SECTION 5: DELETE ENDPOINT TESTS - DELETE /api/employees/{id}
        // ========================================================================

        [Fact]
        public async Task Delete_WithValidId_Returns200Ok()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // ACT
            var result = await controller.Delete(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee deleted", response.Message);
        }

        [Fact]
        public async Task Delete_PassesCorrectCommandToMediator()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            DeleteEmployeeCommand capturedCommand = null;
            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<Unit>, CancellationToken>((cmd, ct) => capturedCommand = cmd as DeleteEmployeeCommand)
                .ReturnsAsync(Unit.Value);

            // ACT
            await controller.Delete(5);

            // ASSERT
            Assert.NotNull(capturedCommand);
            Assert.Equal(5, capturedCommand.Id);
        }

        [Fact]
        public async Task Delete_CallsMediatorSendExactlyOnce()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // ACT
            await controller.Delete(1);

            // ASSERT
            mockMediator.Verify(
                m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()), 
                Times.Once
            );
        }

        [Fact]
        public async Task Delete_WhenMediatorThrowsException_PropagatesException()
        {
            // ARRANGE
            var mockMediator = new Mock<IMediator>();
            var controller = CreateController(mockMediator);

            mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Employee not found"));

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => controller.Delete(999));
        }
    }
}
