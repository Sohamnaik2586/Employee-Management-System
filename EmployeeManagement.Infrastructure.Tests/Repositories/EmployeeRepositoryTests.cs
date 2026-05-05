using Xunit;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Tests.Repositories
{
    /// <summary>
    /// Unit tests for the EmployeeRepository class.
    /// Tests database persistence and data retrieval operations.
    /// 
    /// IMPORTANT: These are INTEGRATION TESTS (not pure unit tests)
    /// - They test the repository with a real (in-memory) database
    /// - Uses Microsoft.EntityFrameworkCore.InMemory for testing
    /// - Tests the full EF Core integration
    /// </summary>
    public class EmployeeRepositoryTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // ========================================================================
        // SECTION 1: ADD EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void Add_WithValidEmployee_PersistsToDatabase()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);
            var employee = new Employee("John Doe", "IT", 50000);

            // ACT
            repository.Add(employee);

            // ASSERT
            var savedEmployee = context.Employees.FirstOrDefault(e => e.Name == "John Doe");
            Assert.NotNull(savedEmployee);
            Assert.Equal("John Doe", savedEmployee.Name);
            Assert.Equal("IT", savedEmployee.Department);
            Assert.Equal(50000, savedEmployee.Salary);
        }

        [Fact]
        public void Add_MultipleEmployees_AllPersist()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee1 = new Employee("John Doe", "IT", 50000);
            var employee2 = new Employee("Jane Smith", "HR", 60000);
            var employee3 = new Employee("Bob Johnson", "Finance", 55000);

            // ACT
            repository.Add(employee1);
            repository.Add(employee2);
            repository.Add(employee3);

            // ASSERT
            var allEmployees = context.Employees.ToList();
            Assert.Equal(3, allEmployees.Count);
            Assert.Contains(allEmployees, e => e.Name == "John Doe");
            Assert.Contains(allEmployees, e => e.Name == "Jane Smith");
            Assert.Contains(allEmployees, e => e.Name == "Bob Johnson");
        }

        // ========================================================================
        // SECTION 2: GET ALL EMPLOYEES TESTS
        // ========================================================================

        [Fact]
        public void GetAll_WithMultipleEmployees_ReturnsAllEmployees()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee1 = new Employee("John Doe", "IT", 50000);
            var employee2 = new Employee("Jane Smith", "HR", 60000);
            repository.Add(employee1);
            repository.Add(employee2);

            // ACT
            var result = repository.GetAll();

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Name == "John Doe");
            Assert.Contains(result, e => e.Name == "Jane Smith");
        }

        [Fact]
        public void GetAll_WithNoEmployees_ReturnsEmptyList()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            // ACT
            var result = repository.GetAll();

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_ReturnsListType()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);
            repository.Add(new Employee("John", "IT", 50000));

            // ACT
            var result = repository.GetAll();

            // ASSERT
            Assert.IsType<List<Employee>>(result);
        }

        // ========================================================================
        // SECTION 3: GET BY ID TESTS
        // ========================================================================

        [Fact]
        public void GetById_WithExistingId_ReturnsEmployee()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee = new Employee("John Doe", "IT", 50000);
            repository.Add(employee);

            var savedEmployee = context.Employees.First(e => e.Name == "John Doe");
            int employeeId = savedEmployee.Id;

            // ACT
            var result = repository.GetById(employeeId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("IT", result.Department);
            Assert.Equal(50000, result.Salary);
        }

        [Fact]
        public void GetById_WithNonExistentId_ReturnsNull()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            // ACT
            var result = repository.GetById(999);

            // ASSERT
            Assert.Null(result);
        }

        [Fact]
        public void GetById_WithMultipleEmployees_ReturnsCorrectOne()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee1 = new Employee("John Doe", "IT", 50000);
            var employee2 = new Employee("Jane Smith", "HR", 60000);
            var employee3 = new Employee("Bob Johnson", "Finance", 55000);
            repository.Add(employee1);
            repository.Add(employee2);
            repository.Add(employee3);

            var jane = context.Employees.First(e => e.Name == "Jane Smith");
            int janeId = jane.Id;

            // ACT
            var result = repository.GetById(janeId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Jane Smith", result.Name);
            Assert.Equal("HR", result.Department);
        }

        // ========================================================================
        // SECTION 4: UPDATE EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void Update_WithValidEmployee_PersistsChanges()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee = new Employee("John Doe", "IT", 50000);
            repository.Add(employee);

            var savedEmployee = context.Employees.First(e => e.Name == "John Doe");
            int employeeId = savedEmployee.Id;

            savedEmployee.SetName("John Smith");
            savedEmployee.SetDepartment("HR");
            savedEmployee.SetSalary(60000);

            // ACT
            repository.Update(savedEmployee);

            // ASSERT
            var updatedEmployee = context.Employees.First(e => e.Id == employeeId);
            Assert.Equal("John Smith", updatedEmployee.Name);
            Assert.Equal("HR", updatedEmployee.Department);
            Assert.Equal(60000, updatedEmployee.Salary);
        }

        [Fact]
        public void Update_OnlyUpdatesSpecifiedEmployee()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee1 = new Employee("John Doe", "IT", 50000);
            var employee2 = new Employee("Jane Smith", "HR", 60000);
            repository.Add(employee1);
            repository.Add(employee2);

            var john = context.Employees.First(e => e.Name == "John Doe");
            int johnId = john.Id;
            john.SetName("John Smith");

            // ACT
            repository.Update(john);

            // ASSERT
            var jane = context.Employees.First(e => e.Name == "Jane Smith");
            Assert.Equal("Jane Smith", jane.Name);
            Assert.Equal("HR", jane.Department);
            Assert.Equal(60000, jane.Salary);
        }

        // ========================================================================
        // SECTION 5: DELETE EMPLOYEE TESTS
        // ========================================================================

        [Fact]
        public void Delete_WithValidId_RemovesEmployee()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee = new Employee("John Doe", "IT", 50000);
            repository.Add(employee);

            var savedEmployee = context.Employees.First(e => e.Name == "John Doe");
            int employeeId = savedEmployee.Id;

            // ACT
            repository.Delete(employeeId);

            // ASSERT
            var deletedEmployee = context.Employees.FirstOrDefault(e => e.Id == employeeId);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public void Delete_WithNonExistentId_DoesNotThrow()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            // ACT & ASSERT
            var exception = Record.Exception(() => repository.Delete(999));
            Assert.Null(exception);
        }

        [Fact]
        public void Delete_OnlyRemovesSpecifiedEmployee()
        {
            // ARRANGE
            using var context = CreateDbContext();
            var repository = new EmployeeRepository(context);

            var employee1 = new Employee("John Doe", "IT", 50000);
            var employee2 = new Employee("Jane Smith", "HR", 60000);
            repository.Add(employee1);
            repository.Add(employee2);

            var john = context.Employees.First(e => e.Name == "John Doe");
            int johnId = john.Id;

            // ACT
            repository.Delete(johnId);

            // ASSERT
            var allEmployees = context.Employees.ToList();
            Assert.Single(allEmployees);
            Assert.Equal("Jane Smith", allEmployees[0].Name);
        }
    }
}
