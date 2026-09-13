using DashboardAdmin.Models;

namespace DashboardAdmin.Services;

public class InMemoryDashboardRepository : IDashboardRepository
{
    private static readonly List<DashboardCard> _cards = new()
    {
        new() { Id = 1, Title = "Выручка",     Value = 125000m, Trend = Trend.Up,     Unit = "руб.", Description = "Общая выручка за период" },
        new() { Id = 2, Title = "Заказы",      Value = 342m,    Trend = Trend.Up,     Unit = "шт.",  Description = "Количество оформленных заказов" },
        new() { Id = 3, Title = "Конверсия",   Value = 4.7m,    Trend = Trend.Stable, Unit = "%",    Description = "Конверсия посетителей в покупателей" },
        new() { Id = 4, Title = "Возвраты",    Value = -18500m, Trend = Trend.Down,   Unit = "руб.", Description = "Сумма возвратов за период" },
        new() { Id = 5, Title = "Средний чек", Value = 3650m,   Trend = Trend.Up,     Unit = "руб.", Description = "Средняя сумма заказа" },
        new() { Id = 6, Title = "Отказы",      Value = 12.3m,   Trend = Trend.Down,   Unit = "%",    Description = "Процент отказов" }
    };

    public IEnumerable<DashboardCard> GetAllCards() => _cards;
    public DashboardCard? GetById(int id) => _cards.FirstOrDefault(c => c.Id == id);
    public IEnumerable<DashboardCard> GetLatest(int count) => _cards.OrderByDescending(c => c.Id).Take(count);
}