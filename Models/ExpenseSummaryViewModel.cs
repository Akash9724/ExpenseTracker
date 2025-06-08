namespace ExpenseTracker.Models
{
    public class ExpenseSummaryViewModel
    {
        public Dictionary<string, decimal> CategoryTotals { get; set; }
        public Dictionary<string, decimal> MonthlyTotals { get; set; }
    }
}