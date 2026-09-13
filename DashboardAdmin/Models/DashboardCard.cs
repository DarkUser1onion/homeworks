namespace DashboardAdmin.Models;

public enum Trend { Up, Down, Stable }

public class DashboardCard
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public Trend Trend { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}