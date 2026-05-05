using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.API.Controllers;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Common;

namespace EmployeeManagement.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the EmployeesController.
    /// Tests API endpoint behavior and HTTP response handling.
    /// Uses IEmployeeService interface for mocking, which is testable with Moq.
    /// </summary>
    public class EmployeesControllerTests
    {
        private EmployeesController CreateController(Mock<IEmployeeService> mockService = null)
        {
            mockService ??= new Mock<IEmployeeService>();
            return new EmployeesController(mockService.Object);
        }

        // ========================================================================
        // SECTION 1: CREATE ENDPOINT TESTS - POST /api/employees
        // ========================================================================

        [Fact]
        public void Create_WithValidDto_Returns200OkResult()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            // ACT
            var result = controller.Create(createDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee created", response.Message);
        }

        [Fact]
        public void Create_CallsServiceCreateEmployeeExactlyOnce()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var createDto = new CreateEmployeeDto
            {
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            // ACT
            controller.Create(createDto);

            // ASSERT
            mockService.Verify(
                s => s.CreateEmployee(It.IsAny<CreateEmployeeDto>()), 
                Times.Once, 
                "Service.CreateEmployee should be called exactly once"
            );
        }

        [Fact]
        public void Create_PassesCorrectDtoToService()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var createDto = new CreateEmployeeDto
            {
                Name = "Jane Smith",
                Department = "HR",
                Salary = 60000
            };

            CreateEmployeeDto capturedDto = null;
            mockService
                .Setup(s => s.CreateEmployee(It.IsAny<CreateEmployeeDto>()))
                .Callback<CreateEmployeeDto>(dto => capturedDto = dto);

            // ACT
            controller.Create(createDto);

            // ASSERT
            Assert.NotNull(capturedDto);
            Assert.Equal("Jane Smith", capturedDto.Name);
            Assert.Equal("HR", capturedDto.Department);
            Assert.Equal(60000, capturedDto.Salary);
        }

        // ========================================================================
        // SECTION 2: GET ALL ENDPOINT TESTS - GET /api/employees
        // ========================================================================

        [Fact]
        public void GetAll_WithEmployeesInDatabase_Returns200OkWithData()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var employees = new List<EmployeeDto>
            {
                new EmployeeDto { Id = 1, Name = "John Doe", Department = "IT", Salary = 50000 },
                new EmployeeDto { Id = 2, Name = "Jane Smith", Department = "HR", Salary = 60000 }
            };
            mockService
                .Setup(s => s.GetEmployees())
                .Returns(employees);

            // ACT
            var result = controller.GetAll();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employees fetched", response.Message);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public void GetAll_WithNoEmployees_Returns200OkWithEmptyList()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            mockService
                .Setup(s => s.GetEmployees())
                .Returns(new List<EmployeeDto>());

            // ACT
            var result = controller.GetAll();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public void GetAll_CallsServiceGetEmployeesExactlyOnce()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            mockService
                .Setup(s => s.GetEmployees())
                .Returns(new List<EmployeeDto>());

            // ACT
            controller.GetAll();

            // ASSERT
            mockService.Verify(
                s => s.GetEmployees(), 
                Times.Once, 
                "Service.GetEmployees should be called exactly once"
            );
        }

        // ========================================================================
        // SECTION 3: GET BY ID ENDPOINT TESTS - GET /api/employees/{id}
        // ========================================================================

        [Fact]
        public void GetById_WithValidId_Returns200OkWithEmployee()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = "John Doe",
                Department = "IT",
                Salary = 50000
            };

            mockService
                .Setup(s => s.GetEmployeeById(1))
                .Returns(employeeDto);

            // ACT
            var result = controller.GetById(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee fetched", response.Message);
        }

        [Fact]
        public void GetById_WithInvalidId_Returns404NotFound()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            mockService
                .Setup(s => s.GetEmployeeById(999))
                .Returns((EmployeeDto)null);

            // ACT
            var result = controller.GetById(999);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Employee not found", response.Message);
        }

        [Fact]
        public void GetById_PassesCorrectIdToService()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            mockService
                .Setup(s => s.GetEmployeeById(It.IsAny<int>()))
                .Returns((EmployeeDto)null);

            // ACT
            controller.GetById(5);

            // ASSERT
            mockService.Verify(
                s => s.GetEmployeeById(5), 
                Times.Once, 
                "Service.GetEmployeeById should be called with ID 5"
            );
        }

        // ========================================================================
        // SECTION 4: UPDATE ENDPOINT TESTS - PUT /api/employees/{id}
        // ========================================================================

        [Fact]
        public void Update_WithValidData_Returns200Ok()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane Doe",
                Department = "HR",
                Salary = 60000
            };

            // ACT
            var result = controller.Update(1, updateDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee updated", response.Message);
        }

        [Fact]
        public void Update_PassesCorrectIdAndDtoToService()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane Doe",
                Department = "HR",
                Salary = 60000
            };

            // ACT
            controller.Update(1, updateDto);

            // ASSERT
            mockService.Verify(
                s => s.UpdateEmployee(1, It.IsAny<CreateEmployeeDto>()), 
                Times.Once
            );
        }

        [Fact]
        public void Update_WhenServiceThrowsException_PropagatesException()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            var updateDto = new CreateEmployeeDto
            {
                Name = "Jane",
                Department = "HR",
                Salary = 60000
            };

            mockService
                .Setup(s => s.UpdateEmployee(999, It.IsAny<CreateEmployeeDto>()))
                .Throws(new Exception("Employee not found"));

            // ACT & ASSERT
            Assert.Throws<Exception>(() => controller.Update(999, updateDto));
        }

        // ========================================================================
        // SECTION 5: DELETE ENDPOINT TESTS - DELETE /api/employees/{id}
        // ========================================================================

        [Fact]
        public void Delete_WithValidId_Returns200Ok()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            // ACT
            var result = controller.Delete(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Employee deleted", response.Message);
        }

        [Fact]
        public void Delete_PassesCorrectIdToService()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            // ACT
            controller.Delete(5);

            // ASSERT
            mockService.Verify(
                s => s.DeleteEmployee(5), 
                Times.Once, 
                "Service.DeleteEmployee should be called with ID 5"
            );
        }

        [Fact]
        public void Delete_CallsServiceDeleteEmployeeExactlyOnce()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            // ACT
            controller.Delete(1);

            // ASSERT
            mockService.Verify(
                s => s.DeleteEmployee(It.IsAny<int>()), 
                Times.Once
            );
        }

        [Fact]
        public void Delete_WhenServiceThrowsException_PropagatesException()
        {
            // ARRANGE
            var mockService = new Mock<IEmployeeService>();
            var controller = CreateController(mockService);

            mockService
                .Setup(s => s.DeleteEmployee(999))
                .Throws(new Exception("Employee not found"));

            // ACT & ASSERT
            Assert.Throws<Exception>(() => controller.Delete(999));
        }
    }
}
