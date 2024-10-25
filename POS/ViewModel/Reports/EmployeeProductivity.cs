namespace POS.ViewModel.Raports
{
    internal class EmployeeProductivity
    {
        public string EmployeeName { get; set; }
        public int OrderCount { get; set; }
        public double TotalAmount { get; internal set; }
    }
}
