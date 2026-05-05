using Xunit;
using Moq;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Tests.Services
{
    /// <summary>
    /// Unit tests for the EmployeeService class.
    /// Tests the business logic layer that coordinates between controllers and repositories.
    /// 
    /// IMPORTANT: These tests use Moq to create mock repositories.
    /// - Mock: A fake object that simulates the real repository
    /// - Moq.Object: The mock object itself (used for dependency injection)
    /// 
    /// AAA Pattern Used with Mocking:
    /// - ARRANGE: Create mocks and set up expectations
    /// - ACT: Call the service method
    /// - ASSERT: Verify behavior and mock interactions
    /// </summary>
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly EmployeeService _service;

        /// <summary>
        /// Constructor - Runs before EACH test method
        /// 
        /// WHY: 
        /// - Each test gets a fresh mock (no data leakage between tests)
        /// - Ensures test isolation and independence
        /// - Follows AAA pattern principle of clean test state
        /// </summary>
        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _service = new EmployeeService(_mockRepository.Object);
        }

        // ========================================================================
        // SECTION 1: CREATE EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void CreateEmployee_WithValidDto_CallsRepositoryAdd()
        {
            // ARRANGE
            var createEmployeeDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            // ACT
            _service.CreateEmployee(createEmployeeDto);

            // ASSERT
            _mockRepository.Verify(
                r => r.Add(It.IsAny<Employee>()), 
                Times.Once, 
                "Repository.Add should be called exactly once when creating an employee"
            );
        }

        [Fact]
        public void CreateEmployee_PassesCorrectEmployeeDataToRepository()
        {
            // ARRANGE
            var createEmployeeDto = new CreateEmployeeDto
            {
                Name = "Jane Smith",
                Department = "HR",
                Salary = 60000
            };

            Employee capturedEmployee = null;
            _mockRepository
                .Setup(r => r.Add(It.IsAny<Employee>()))
                .Callback<Employee>(emp => capturedEmployee = emp);

            // ACT
            _service.CreateEmployee(createEmployeeDto);

            // ASSERT
            Assert.NotNull(capturedEmployee);
            Assert.Equal("Jane Smith", capturedEmployee.Name);
            Assert.Equal("HR", capturedEmployee.Department);
            Assert.Equal(60000, capturedEmployee.Salary);
        }

        [Fact]
        public void CreateEmployee_WithEmptyName_ThrowsException()
        {
            // ARRANGE
            var createEmployeeDto = new CreateEmployeeDto
            {
                Name = "",
                Department = "IT",
                Salary = 50000
            };

            // ACT & ASSERT
            Assert.Throws<ArgumentException>(() => _service.CreateEmployee(createEmployeeDto));
            
            _mockRepository.Verify(
                r => r.Add(It.IsAny<Employee>()), 
                Times.Never, 
                "Repository.Add should NOT be called when domain validation fails"
            );
        }

        // ========================================================================
        // SECTION 2: GET EMPLOYEES TESTS
        // ========================================================================

        [Fact]
        public void GetEmployees_ReturnsAllEmployeesAsDtos()
        {
            // ARRANGE
            var mockEmployees = new List<Employee>
            {
                new Employee("John Doe", "IT", 50000),
                new Employee("Jane Smith", "HR", 60000)
            };

            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(mockEmployees);

            // ACT
            var result = _service.GetEmployees();

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
        public void GetEmployees_WhenNoEmployeesExist_ReturnsEmptyList()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(new List<Employee>());

            // ACT
            var result = _service.GetEmployees();

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetEmployees_CallsRepositoryGetAllExactlyOnce()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetAll())
                .Returns(new List<Employee>());

            // ACT
            _service.GetEmployees();

            // ASSERT
            _mockRepository.Verify(
                r => r.GetAll(), 
                Times.Once, 
                "Repository.GetAll should be called exactly once"
            );
        }

        // ========================================================================
        // SECTION 3: GET EMPLOYEE BY ID TESTS
        // ========================================================================

        [Fact]
        public void GetEmployeeById_WithValidId_ReturnsEmployeeDto()
        {
            // ARRANGE
            var mockEmployee = new Employee("John Doe", "IT", 50000);
            
            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(mockEmployee);

            // ACT
            var result = _service.GetEmployeeById(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("IT", result.Department);
            Assert.Equal(50000, result.Salary);
        }

        [Fact]
        public void GetEmployeeById_WithInvalidId_ReturnsNull()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            // ACT
            var result = _service.GetEmployeeById(999);

            // ASSERT
            Assert.Null(result);
        }

        [Fact]
        public void GetEmployeeById_PassesCorrectIdToRepository()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Employee)null);

            // ACT
            _service.GetEmployeeById(5);

            // ASSERT
            _mockRepository.Verify(
                r => r.GetById(5), 
                Times.Once, 
                "Repository.GetById should be called with ID 5"
            );
        }

        // ========================================================================
        // SECTION 4: UPDATE EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void UpdateEmployee_WithValidData_UpdatesSuccessfully()
        {
            // ARRANGE
            var originalEmployee = new Employee("John Doe", "IT", 50000);
            
            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(originalEmployee);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane Doe",
                Department = "HR",
                Salary = 60000
            };

            // ACT
            _service.UpdateEmployee(1, updateDto);

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
        public void UpdateEmployee_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane",
                Department = "HR",
                Salary = 60000
            };

            // ACT & ASSERT
            Assert.Throws<Exception>(() => _service.UpdateEmployee(999, updateDto));

            _mockRepository.Verify(
                r => r.Update(It.IsAny<Employee>()), 
                Times.Never, 
                "Repository.Update should NOT be called if employee not found"
            );
        }

        [Fact]
        public void UpdateEmployee_WithEmptyName_ThrowsException()
        {
            // ARRANGE
            var originalEmployee = new Employee("John Doe", "IT", 50000);
            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(originalEmployee);

            var updateDto = new CreateEmployeeDto
            {
                Name = "",
                Department = "HR",
                Salary = 60000
            };

            // ACT & ASSERT
            Assert.Throws<ArgumentException>(() => _service.UpdateEmployee(1, updateDto));

            _mockRepository.Verify(
                r => r.Update(It.IsAny<Employee>()), 
                Times.Never, 
                "Repository.Update should NOT be called when domain validation fails"
            );
        }

        // ========================================================================
        // SECTION 5: DELETE EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void DeleteEmployee_WithValidId_DeletesSuccessfully()
        {
            // ARRANGE
            var employeeToDelete = new Employee("John Doe", "IT", 50000);
            
            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(employeeToDelete);

            // ACT
            _service.DeleteEmployee(1);

            // ASSERT
            _mockRepository.Verify(
                r => r.Delete(1), 
                Times.Once, 
                "Repository.Delete should be called with ID 1"
            );
        }

        [Fact]
        public void DeleteEmployee_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetById(999))
                .Returns((Employee)null);

            // ACT & ASSERT
            Assert.Throws<Exception>(() => _service.DeleteEmployee(999));

            _mockRepository.Verify(
                r => r.Delete(It.IsAny<int>()), 
                Times.Never, 
                "Repository.Delete should NOT be called if employee not found"
            );
        }

        [Fact]
        public void DeleteEmployee_CallsRepositoryDeleteExactlyOnce()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            _mockRepository
                .Setup(r => r.GetById(1))
                .Returns(employee);

            // ACT
            _service.DeleteEmployee(1);

            // ASSERT
            _mockRepository.Verify(
                r => r.Delete(1), 
                Times.Once, 
                "Repository.Delete should be called exactly once"
            );
        }
    }
}
