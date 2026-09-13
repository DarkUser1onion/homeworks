using DashboardAdmin.Models;
using DashboardAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashboardAdmin.ViewComponents;

public class TrendSummaryViewComponent : ViewComponent
{
    private readonly IDashboardRepository _repository;
    public TrendSummaryViewComponent(IDashboardRepository repository) => _repository = repository;

    public IViewComponentResult Invoke()
    {
        var summaries = _repository.GetAllCards()
            .GroupBy(c => c.Trend)
            .Select(g => new TrendSummaryItem
            {
                Trend = g.Key,
                Count = g.Count(),
                Sum = g.Sum(c => c.Value)
            })
            .ToList();

        return View(summaries);
    }
}

public class TrendSummaryItem
{
    public Trend Trend { get; set; }
    public int Count { get; set; }
    public decimal Sum { get; set; }
}
