namespace DataAccess.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? HireDate { get; set; }
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required bool IsUserLoggedIn { get; set; }
    }
}