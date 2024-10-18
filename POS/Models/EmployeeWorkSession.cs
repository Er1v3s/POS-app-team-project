namespace POS.Models
{
    public class EmployeeWorkSession
    {
        public int WorkSessionId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? WorkingTimeFrom { get; set; }
        public string? WorkingTimeTo { get; set; }
        public string? WorkingTimeSummary { get; set; }
    }
}
