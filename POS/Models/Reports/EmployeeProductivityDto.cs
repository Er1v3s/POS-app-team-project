namespace POS.Models.Reports
{
    internal class EmployeeProductivityDto
    {
        public string EmployeeName { get; set; }
        public int OrderCount { get; set; }
        public double TotalAmount { get; internal set; }
    }
}
