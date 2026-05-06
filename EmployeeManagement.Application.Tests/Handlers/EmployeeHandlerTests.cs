using Xunit;
using Moq;
using EmployeeManagement.Application.Handlers;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Commands;
using EmployeeManagement.Application.Queries;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Tests.Handlers
{
    /// <summary>
    /// Unit tests for CQRS Command and Query Handlers.
    /// Tests the business logic layer that coordinates between controllers and repositories.
    /// 
    /// IMPORTANT: These tests use Moq to create mock repositories.
    /// - Mock: A fake object that simulates the real repository
    /// - Moq.Object: The mock object itself (used for dependency injection)
    /// 
    /// AAA Pattern Used with Mocking:
    /// - ARRANGE: Create mocks and set up expectations
    /// - ACT: Call the handler
    /// - ASSERT: Verify behavior and mock interactions
    /// </summary>
    public class EmployeeHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;

        /// <summary>
        /// Constructor - Runs before EACH test method
        /// 
        /// WHY: 
        /// - Each test gets a fresh mock (no data leakage between tests)
        /// - Ensures test isolation and independence
        /// - Follows AAA pattern principle of clean test state
        /// </summary>
        public EmployeeHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
        }

        // ========================================================================
        // SECTION 1: CREATE EMPLOYEE COMMAND TESTS
        // ========================================================================

        [Fact]
        public async Task CreateEmployeeCommandHandler_WithValidCommand_CallsRepositoryAdd()
        {
            // ARRANGE
            var handler = new CreateEmployeeCommandHandler(_mockRepository.Object);
            var command = new CreateEmployeeCommand("John Doe", "IT", 50000);

            // ACT
            await handler.Handle(command, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.Add(It.IsAny<Employee>()), 
                Times.Once, 
                "Repository.Add should be called exactly once when creating an employee"
            );
        }

        [Fact]
        public async Task CreateEmployeeCommandHandler_PassesCorrectEmployeeDataToRepository()
        {
            // ARRANGE
            var handler = new CreateEmployeeCommandHandler(_mockRepository.Object);
            var command = new CreateEmployeeCommand("Jane Smith", "HR", 60000);

            Employee capturedEmployee = null;
            _mockRepository
                .Setup(r => r.Add(It.IsAny<Employee>()))
                .Callback<Employee>(emp => capturedEmployee = emp);

            // ACT
            await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(capturedEmployee);
            Assert.Equal("Jane Smith", capturedEmployee.Name);
            Assert.Equal("HR", capturedEmployee.Department);
            Assert.Equal(60000, capturedEmployee.Salary);
        }

        [Fact]
        public async Task CreateEmployeeCommandHandler_ReturnsEmployeeId()
        {
            // ARRANGE
            var handler = new CreateEmployeeCommandHandler(_mockRepository.Object);
            var command = new CreateEmployeeCommand("John Doe", "IT", 50000);

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.IsType<int>(result);
        }

        // ========================================================================
        // SECTION 2: GET ALL EMPLOYEES QUERY TESTS
        // ========================================================================

        [Fact]
        public async Task GetAllEmployeesQueryHandler_ReturnsAllEmployeesAsDtos()
        {
            // ARRANGE
            var handler = new GetAllEmployeesQueryHandler(_mockRepository.Object);
            var query = new GetAllEmployeesQuery();

            var mockEmployees = new List<Employee>
            {
                new Employee("John Doe", "IT", 50000),
                new Employee("Jane Smith", "HR", 60000)
            };

            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(mockEmployees);

            // ACT
            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].Name);
            Assert.Equal("IT", result[0].Department);
            Assert.Equal(50000, result[0].Salary);
            Assert.Equal("Jane Smith", result[1].Name);
            Assert.Equal("HR", result[1].Department);
            Assert.Equal(60000, result[1].Salary);
        }

        [Fact]
        public async Task GetAllEmployeesQueryHandler_WhenNoEmployeesExist_ReturnsEmptyList()
        {
            // ARRANGE
            var handler = new GetAllEmployeesQueryHandler(_mockRepository.Object);
            var query = new GetAllEmployeesQuery();

            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(new List<Employee>());

            // ACT
            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllEmployeesQueryHandler_CallsRepositoryGetAllExactlyOnce()
        {
            // ARRANGE
            var handler = new GetAllEmployeesQueryHandler(_mockRepository.Object);
            var query = new GetAllEmployeesQuery();

            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(new List<Employee>());

            // ACT
            await handler.Handle(query, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.GetAll(), 
                Times.Once, 
                "Repository.GetAll should be called exactly once"
            );
        }

        // ========================================================================
        // SECTION 3: GET EMPLOYEE BY ID QUERY TESTS
        // ========================================================================

        [Fact]
        public async Task GetEmployeeByIdQueryHandler_WithValidId_ReturnsEmployeeDto()
        {
            // ARRANGE
            var handler = new GetEmployeeByIdQueryHandler(_mockRepository.Object);
            var query = new GetEmployeeByIdQuery(1);

            var mockEmployee = new Employee("John Doe", "IT", 50000);

            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(mockEmployee);

            // ACT
            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("IT", result.Department);
            Assert.Equal(50000, result.Salary);
        }

        [Fact]
        public async Task GetEmployeeByIdQueryHandler_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            var handler = new GetEmployeeByIdQueryHandler(_mockRepository.Object);
            var query = new GetEmployeeByIdQuery(999);

            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task GetEmployeeByIdQueryHandler_PassesCorrectIdToRepository()
        {
            // ARRANGE
            var handler = new GetEmployeeByIdQueryHandler(_mockRepository.Object);
            var query = new GetEmployeeByIdQuery(5);

            var mockEmployee = new Employee("John Doe", "IT", 50000);
            _mockRepository
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(mockEmployee);

            // ACT
            await handler.Handle(query, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.GetById(5), 
                Times.Once, 
                "Repository.GetById should be called with ID 5"
            );
        }

        // ========================================================================
        // SECTION 4: UPDATE EMPLOYEE COMMAND TESTS
        // ========================================================================

        [Fact]
        public async Task UpdateEmployeeCommandHandler_WithValidData_UpdatesSuccessfully()
        {
            // ARRANGE
            var handler = new UpdateEmployeeCommandHandler(_mockRepository.Object);
            var command = new UpdateEmployeeCommand(1, "Jane Doe", "HR", 60000);

            var originalEmployee = new Employee("John Doe", "IT", 50000);

            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(originalEmployee);

            // ACT
            await handler.Handle(command, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.Update(It.IsAny<Employee>()), 
                Times.Once, 
                "Repository.Update should be called once"
            );

            Assert.Equal("Jane Doe", originalEmployee.Name);
            Assert.Equal("HR", originalEmployee.Department);
            Assert.Equal(60000, originalEmployee.Salary);
        }

        [Fact]
        public async Task UpdateEmployeeCommandHandler_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            var handler = new UpdateEmployeeCommandHandler(_mockRepository.Object);
            var command = new UpdateEmployeeCommand(999, "Jane", "HR", 60000);

            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(
                r => r.Update(It.IsAny<Employee>()), 
                Times.Never, 
                "Repository.Update should NOT be called if employee not found"
            );
        }

        // ========================================================================
        // SECTION 5: DELETE EMPLOYEE COMMAND TESTS
        // ========================================================================

        [Fact]
        public async Task DeleteEmployeeCommandHandler_WithValidId_DeletesSuccessfully()
        {
            // ARRANGE
            var handler = new DeleteEmployeeCommandHandler(_mockRepository.Object);
            var command = new DeleteEmployeeCommand(1);

            var employeeToDelete = new Employee("John Doe", "IT", 50000);

            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(employeeToDelete);

            // ACT
            await handler.Handle(command, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.Delete(1), 
                Times.Once, 
                "Repository.Delete should be called with ID 1"
            );
        }

        [Fact]
        public async Task DeleteEmployeeCommandHandler_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            var handler = new DeleteEmployeeCommandHandler(_mockRepository.Object);
            var command = new DeleteEmployeeCommand(999);

            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(
                r => r.Delete(It.IsAny<int>()), 
                Times.Never, 
                "Repository.Delete should NOT be called if employee not found"
            );
        }

        [Fact]
        public async Task DeleteEmployeeCommandHandler_CallsRepositoryDeleteExactlyOnce()
        {
            // ARRANGE
            var handler = new DeleteEmployeeCommandHandler(_mockRepository.Object);
            var command = new DeleteEmployeeCommand(1);

            var employeeToDelete = new Employee("John Doe", "IT", 50000);

            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(employeeToDelete);

            // ACT
            await handler.Handle(command, CancellationToken.None);

            // ASSERT
            _mockRepository.Verify(
                r => r.Delete(1), 
                Times.Once
            );
        }
    }
}
