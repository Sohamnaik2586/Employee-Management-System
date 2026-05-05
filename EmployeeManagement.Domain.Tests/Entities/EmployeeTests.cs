using Xunit;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Tests.Entities
{
    /// <summary>
    /// Unit tests for the Employee entity.
    /// Tests focus on validating business rules enforced at the domain level.
    /// 
    /// AAA Pattern Used:
    /// - ARRANGE: Set up test data and expected conditions
    /// - ACT: Execute the code being tested
    /// - ASSERT: Verify the results match expectations
    /// </summary>
    public class EmployeeTests
    {
        // ========================================================================
        // SECTION 1: CONSTRUCTOR TESTS - Valid Employee Creation
        // ========================================================================

        /// <summary>
        /// TEST CASE: Employee Constructor With Valid Data
        /// PURPOSE: Verify that an employee is created successfully when all valid data is provided
        /// 
        /// SCENARIO:
        /// - Input: Valid name, department, and salary
        /// - Expected: Employee object created with correct property values
        /// 
        /// WHY THIS TEST:
        /// - Ensures the happy path works
        /// - Verifies all properties are set correctly
        /// - Foundation test for the entity
        /// </summary>
        [Fact]
        public void Constructor_WithValidData_CreatesEmployeeSuccessfully()
        {
            // ARRANGE: Set up test data (prepare inputs)
            string testName = "John Doe";
            string testDepartment = "IT";
            decimal testSalary = 50000;

            // ACT: Execute the code being tested (create the employee)
            var employee = new Employee(testName, testDepartment, testSalary);

            // ASSERT: Verify the results (check if creation was successful)
            // Verify that the employee object was created (not null)
            Assert.NotNull(employee);
            
            // Verify that Name property has the correct value
            Assert.Equal(testName, employee.Name);
            
            // Verify that Department property has the correct value
            Assert.Equal(testDepartment, employee.Department);
            
            // Verify that Salary property has the correct value
            Assert.Equal(testSalary, employee.Salary);
        }

        /// <summary>
        /// TEST CASE: Constructor With Different Valid Values
        /// PURPOSE: Test with different valid data to ensure flexibility
        /// 
        /// WHY THIS TEST:
        /// - Verifies constructor works with various valid inputs
        /// - Tests boundary values (minimum length names, different departments)
        /// </summary>
        [Fact]
        public void Constructor_WithDifferentValidData_CreatesEmployeeSuccessfully()
        {
            // ARRANGE: Use different valid test data
            string testName = "Jane Smith";
            string testDepartment = "HR";
            decimal testSalary = 60000.50m; // Decimal with 2 places as per schema

            // ACT: Create employee with different data
            var employee = new Employee(testName, testDepartment, testSalary);

            // ASSERT: Verify this data set also works correctly
            Assert.Equal(testName, employee.Name);
            Assert.Equal(testDepartment, employee.Department);
            Assert.Equal(testSalary, employee.Salary);
        }

        // ========================================================================
        // SECTION 2: NAME VALIDATION TESTS - SetName Method
        // ========================================================================

        /// <summary>
        /// TEST CASE: SetName With Empty String
        /// PURPOSE: Verify that setting an empty name throws ArgumentException
        /// 
        /// SCENARIO:
        /// - Input: Empty string ""
        /// - Expected: ArgumentException thrown with appropriate message
        /// 
        /// WHY THIS TEST:
        /// - Ensures business rule: Name cannot be empty
        /// - Validates error handling works correctly
        /// - Prevents invalid data from being persisted
        /// </summary>
        [Fact]
        public void SetName_WithEmptyString_ThrowsArgumentException()
        {
            // ARRANGE: Create a valid employee first
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT: Try to set empty name and catch the exception
            // Assert.Throws<T> verifies that calling the code throws the expected exception type
            var exception = Assert.Throws<ArgumentException>(() => employee.SetName(""));
            
            // Verify the error message contains the expected text
            // This helps developers understand why the exception occurred
            Assert.Contains("Name cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetName With Only Whitespace
        /// PURPOSE: Verify that whitespace-only names are rejected (same as empty)
        /// 
        /// WHY THIS TEST:
        /// - Whitespace like "   " is technically a string but logically empty
        /// - Business rule should treat it as invalid
        /// - Tests string.IsNullOrWhiteSpace() validation
        /// </summary>
        [Fact]
        public void SetName_WithOnlyWhitespace_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT: Whitespace should be rejected like empty strings
            var exception = Assert.Throws<ArgumentException>(() => employee.SetName("   "));
            Assert.Contains("Name cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetName With Null Value
        /// PURPOSE: Verify that null names are rejected
        /// 
        /// WHY THIS TEST:
        /// - Null is a special invalid value that needs handling
        /// - Tests string.IsNullOrWhiteSpace() with null input
        /// </summary>
        [Fact]
        public void SetName_WithNullValue_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT: Null should trigger the validation error
            var exception = Assert.Throws<ArgumentException>(() => employee.SetName(null));
            Assert.Contains("Name cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetName Exceeding Maximum Length (50 characters)
        /// PURPOSE: Verify that names longer than 50 characters are rejected
        /// 
        /// SCENARIO:
        /// - Input: String with 51 characters (exceeds max of 50)
        /// - Expected: ArgumentException thrown
        /// 
        /// WHY THIS TEST:
        /// - Database schema limits Name to VARCHAR(50)
        /// - Must enforce this at domain level to prevent data truncation
        /// - Tests the upper boundary of name length
        /// </summary>
        [Fact]
        public void SetName_ExceedsMaxLength_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Create a string with exactly 51 characters (1 over the 50 char limit)
            // string constructor with char and count: new string('A', 51) creates "AAA...A" (51 A's)
            string nameTooLong = new string('A', 51);

            // ACT & ASSERT: Names exceeding 50 characters should be rejected
            var exception = Assert.Throws<ArgumentException>(() => employee.SetName(nameTooLong));
            Assert.Contains("Name cannot exceed 50 characters", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetName At Maximum Length (50 characters)
        /// PURPOSE: Verify that exactly 50 character names are accepted
        /// 
        /// WHY THIS TEST:
        /// - Tests the boundary: 50 chars is valid, 51 is invalid
        /// - Ensures the validation uses > not >=
        /// </summary>
        [Fact]
        public void SetName_AtMaxLength_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Create a string with exactly 50 characters (at the limit, not over)
            string nameAtMaxLength = new string('A', 50);

            // ACT: Set the name with exactly 50 characters
            employee.SetName(nameAtMaxLength);

            // ASSERT: Verify the name was set successfully (no exception thrown)
            Assert.Equal(nameAtMaxLength, employee.Name);
        }

        /// <summary>
        /// TEST CASE: SetName With Valid Name Updates Successfully
        /// PURPOSE: Verify that valid names can be set/updated after creation
        /// 
        /// WHY THIS TEST:
        /// - Tests the positive case: updating an existing employee's name
        /// - Verifies the property change is reflected
        /// </summary>
        [Fact]
        public void SetName_WithValidName_UpdatesSuccessfully()
        {
            // ARRANGE: Create employee with initial name
            var employee = new Employee("John", "IT", 50000);
            string newName = "Jane Doe";

            // ACT: Update the employee's name
            employee.SetName(newName);

            // ASSERT: Verify the name was updated
            Assert.Equal(newName, employee.Name);
        }

        // ========================================================================
        // SECTION 3: DEPARTMENT VALIDATION TESTS - SetDepartment Method
        // ========================================================================

        /// <summary>
        /// TEST CASE: SetDepartment With Empty String
        /// PURPOSE: Verify that empty department names are rejected
        /// 
        /// WHY THIS TEST:
        /// - Business rule: Department cannot be empty
        /// - Similar to Name validation but different field
        /// </summary>
        [Fact]
        public void SetDepartment_WithEmptyString_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT: Empty department should be rejected
            var exception = Assert.Throws<ArgumentException>(() => employee.SetDepartment(""));
            Assert.Contains("Department cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetDepartment With Only Whitespace
        /// PURPOSE: Verify whitespace-only departments are rejected
        /// </summary>
        [Fact]
        public void SetDepartment_WithOnlyWhitespace_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT
            var exception = Assert.Throws<ArgumentException>(() => employee.SetDepartment("   "));
            Assert.Contains("Department cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetDepartment With Null Value
        /// PURPOSE: Verify that null departments are rejected
        /// </summary>
        [Fact]
        public void SetDepartment_WithNullValue_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT
            var exception = Assert.Throws<ArgumentException>(() => employee.SetDepartment(null));
            Assert.Contains("Department cannot be empty", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetDepartment Exceeding Maximum Length (25 characters)
        /// PURPOSE: Verify that departments longer than 25 characters are rejected
        /// 
        /// SCENARIO:
        /// - Input: String with 26 characters (exceeds max of 25)
        /// - Expected: ArgumentException thrown
        /// 
        /// WHY THIS TEST:
        /// - Database schema limits Department to VARCHAR(25)
        /// - Must enforce this at domain level
        /// </summary>
        [Fact]
        public void SetDepartment_ExceedsMaxLength_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Create a string with 26 characters (exceeds the 25 char limit)
            string departmentTooLong = new string('X', 26);

            // ACT & ASSERT: Departments exceeding 25 characters should be rejected
            var exception = Assert.Throws<ArgumentException>(() => employee.SetDepartment(departmentTooLong));
            Assert.Contains("Department cannot exceed 25 characters", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetDepartment At Maximum Length (25 characters)
        /// PURPOSE: Verify that exactly 25 character departments are accepted
        /// </summary>
        [Fact]
        public void SetDepartment_AtMaxLength_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Create a string with exactly 25 characters (at the limit)
            string departmentAtMaxLength = new string('Y', 25);

            // ACT: Set the department with exactly 25 characters
            employee.SetDepartment(departmentAtMaxLength);

            // ASSERT: Verify the department was set successfully
            Assert.Equal(departmentAtMaxLength, employee.Department);
        }

        /// <summary>
        /// TEST CASE: SetDepartment With Valid Department Updates Successfully
        /// PURPOSE: Verify that valid departments can be set/updated
        /// </summary>
        [Fact]
        public void SetDepartment_WithValidDepartment_UpdatesSuccessfully()
        {
            // ARRANGE: Create employee with initial department
            var employee = new Employee("John", "IT", 50000);
            string newDepartment = "HR";

            // ACT: Update the employee's department
            employee.SetDepartment(newDepartment);

            // ASSERT: Verify the department was updated
            Assert.Equal(newDepartment, employee.Department);
        }

        // ========================================================================
        // SECTION 4: SALARY VALIDATION TESTS - SetSalary Method
        // ========================================================================

        /// <summary>
        /// TEST CASE: SetSalary With Negative Value
        /// PURPOSE: Verify that negative salaries are rejected
        /// 
        /// SCENARIO:
        /// - Input: Negative decimal (-1000)
        /// - Expected: ArgumentException thrown
        /// 
        /// WHY THIS TEST:
        /// - Business rule: Salary cannot be negative
        /// - Prevents nonsensical negative compensation
        /// - Critical validation for financial data
        /// </summary>
        [Fact]
        public void SetSalary_WithNegativeValue_ThrowsArgumentException()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT & ASSERT: Negative salaries should be rejected
            var exception = Assert.Throws<ArgumentException>(() => employee.SetSalary(-1000));
            Assert.Contains("Salary cannot be negative", exception.Message);
        }

        /// <summary>
        /// TEST CASE: SetSalary With Zero Value
        /// PURPOSE: Verify that zero salary is accepted (valid edge case)
        /// 
        /// WHY THIS TEST:
        /// - Zero salary might be valid (unpaid intern, volunteer)
        /// - Tests boundary: validation is < 0, not <= 0
        /// - Important distinction: rejection point is at negative, not at zero
        /// </summary>
        [Fact]
        public void SetSalary_WithZero_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT: Set salary to zero
            employee.SetSalary(0);

            // ASSERT: Verify zero salary is accepted
            Assert.Equal(0, employee.Salary);
        }

        /// <summary>
        /// TEST CASE: SetSalary With Large Valid Value
        /// PURPOSE: Verify that large salary values are accepted
        /// 
        /// WHY THIS TEST:
        /// - Database schema: DECIMAL(10,2) supports up to 99,999,999.99
        /// - Tests that reasonable upper ranges work
        /// - Ensures no accidental upper limit on valid salaries
        /// </summary>
        [Fact]
        public void SetSalary_WithLargeValidValue_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Use a large but valid salary value (within DECIMAL(10,2) range)
            decimal largeSalary = 9999999.99m;

            // ACT: Set a large salary
            employee.SetSalary(largeSalary);

            // ASSERT: Verify large salary is accepted
            Assert.Equal(largeSalary, employee.Salary);
        }

        /// <summary>
        /// TEST CASE: SetSalary With Decimal Precision
        /// PURPOSE: Verify salary precision is maintained correctly
        /// 
        /// SCENARIO:
        /// - Input: Salary with decimal places (50000.50)
        /// - Expected: Decimal value stored exactly
        /// 
        /// WHY THIS TEST:
        /// - Database schema requires DECIMAL(10,2) for financial accuracy
        /// - Tests that decimal places are preserved
        /// - Prevents floating-point rounding errors
        /// </summary>
        [Fact]
        public void SetSalary_WithDecimalValue_MaintainsPrecision()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);
            
            // Use a salary with 2 decimal places
            decimal salaryWithDecimals = 75000.50m;

            // ACT: Set the salary with decimal places
            employee.SetSalary(salaryWithDecimals);

            // ASSERT: Verify the exact decimal value is preserved (no rounding)
            Assert.Equal(salaryWithDecimals, employee.Salary);
        }

        /// <summary>
        /// TEST CASE: SetSalary Updates Existing Salary Successfully
        /// PURPOSE: Verify that existing salaries can be updated
        /// </summary>
        [Fact]
        public void SetSalary_WithValidValue_UpdatesSuccessfully()
        {
            // ARRANGE: Create employee with initial salary
            var employee = new Employee("John", "IT", 50000);
            decimal newSalary = 65000;

            // ACT: Update the employee's salary
            employee.SetSalary(newSalary);

            // ASSERT: Verify the salary was updated
            Assert.Equal(newSalary, employee.Salary);
        }

        // ========================================================================
        // SECTION 5: MULTIPLE UPDATES TEST - Changing Multiple Properties
        // ========================================================================

        /// <summary>
        /// TEST CASE: Update Multiple Properties Sequentially
        /// PURPOSE: Verify that multiple property updates work together correctly
        /// 
        /// WHY THIS TEST:
        /// - Simulates real-world scenario: updating employee details
        /// - Ensures changes to one property don't affect others
        /// - Validates object state consistency across updates
        /// </summary>
        [Fact]
        public void UpdateMultipleProperties_Succeeds()
        {
            // ARRANGE: Create employee with initial values
            var employee = new Employee("John Doe", "IT", 50000);

            // ACT: Update all three properties sequentially
            employee.SetName("Jane Smith");
            employee.SetDepartment("HR");
            employee.SetSalary(60000);

            // ASSERT: Verify all properties were updated correctly
            Assert.Equal("Jane Smith", employee.Name);
            Assert.Equal("HR", employee.Department);
            Assert.Equal(60000, employee.Salary);
        }

        // ========================================================================
        // SECTION 6: EDGE CASES AND SPECIAL SCENARIOS
        // ========================================================================

        /// <summary>
        /// TEST CASE: Name With Minimum Length
        /// PURPOSE: Verify that minimum length names are accepted
        /// 
        /// WHY THIS TEST:
        /// - DTO has [MinLength(2)] but domain allows any non-empty
        /// - Tests single character names are valid at domain level
        /// </summary>
        [Fact]
        public void SetName_WithSingleCharacter_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT: Set a single character name
            employee.SetName("A");

            // ASSERT: Verify single character is accepted
            Assert.Equal("A", employee.Name);
        }

        /// <summary>
        /// TEST CASE: Name With Special Characters
        /// PURPOSE: Verify that special characters in names are accepted
        /// 
        /// WHY THIS TEST:
        /// - Real names may contain apostrophes, hyphens, etc.
        /// - Domain should not restrict valid character types
        /// </summary>
        [Fact]
        public void SetName_WithSpecialCharacters_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT: Set a name with special characters
            employee.SetName("Mary-Jane O'Connor");

            // ASSERT: Verify special characters are accepted
            Assert.Equal("Mary-Jane O'Connor", employee.Name);
        }

        /// <summary>
        /// TEST CASE: Department With Single Character
        /// PURPOSE: Verify that single character departments are accepted
        /// </summary>
        [Fact]
        public void SetDepartment_WithSingleCharacter_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT: Set a single character department
            employee.SetDepartment("R");

            // ASSERT: Verify single character is accepted
            Assert.Equal("R", employee.Department);
        }

        /// <summary>
        /// TEST CASE: Salary With Very Small Decimal Value
        /// PURPOSE: Verify that cents (smallest unit) are handled correctly
        /// 
        /// WHY THIS TEST:
        /// - DECIMAL(10,2) supports values like 0.01
        /// - Ensures financial precision at smallest granularity
        /// </summary>
        [Fact]
        public void SetSalary_WithSmallDecimalValue_Succeeds()
        {
            // ARRANGE
            var employee = new Employee("John", "IT", 50000);

            // ACT: Set salary to a very small value (1 cent)
            employee.SetSalary(0.01m);

            // ASSERT: Verify small decimal value is accepted
            Assert.Equal(0.01m, employee.Salary);
        }
    }
}
