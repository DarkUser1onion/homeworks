namespace DashboardAdmin.Models;

public class DashboardViewModel
{
    public IEnumerable<DashboardCard> Cards { get; set; } = new List<DashboardCard>();
    public decimal TotalValue { get; set; }
    public int CardCount { get; set; }
    public string ReportPeriod { get; set; } = string.Empty;
    public string Currency { get; set; } = "RUB";
}