using Microsoft.AspNetCore.Mvc;
using ShopCatalog.Models;

namespace ShopCatalog.Controllers;

public class CatalogController : Controller
{
    // Пока — статический список, потом при желании вынесем в репозиторий.
    private static readonly List<Product> _products = new()
    {
        new() { Id = 1, Name = "Наушники Sony WH-1000XM5", Description = "Беспроводные, шумоподавление",
                Price = 29990m, OldPrice = 34990m, IsHit = true, Category = "Электроника",
                ImageUrl = "https://picsum.photos/seed/headphones/400/300" },
        new() { Id = 2, Name = "Книга \"Чистый код\"", Description = "Роберт Мартин, классика",
                Price = 1890m, IsHit = false, Category = "Книги",
                ImageUrl = "https://picsum.photos/seed/book/400/300" },
        new() { Id = 3, Name = "Футболка оверсайз", Description = "Хлопок 100%, унисекс",
                Price = 1490m, OldPrice = 2490m, IsHit = true, Category = "Одежда",
                ImageUrl = "https://picsum.photos/seed/tshirt/400/300" },
        new() { Id = 4, Name = "Механическая клавиатура", Description = "Переключатели Cherry MX",
                Price = 7490m, Category = "Электроника",
                ImageUrl = "https://picsum.photos/seed/keyboard/400/300" },
        new() { Id = 5, Name = "Кружка «Программист»", Description = "Керамика, 350 мл",
                Price = 590m, IsHit = false, Category = "Дом",
                ImageUrl = "https://picsum.photos/seed/mug/400/300" },
        new() { Id = 6, Name = "Рюкзак городской", Description = "Водоотталкивающий, 25 л",
                Price = 3490m, OldPrice = 4290m, Category = "Одежда",
                ImageUrl = "https://picsum.photos/seed/backpack/400/300" }
    };

    public IActionResult Index(string? category)
    {
        ViewData["Title"] = "Каталог товаров";

        ViewBag.Categories = _products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        ViewBag.SelectedCategory = category;

        var items = string.IsNullOrEmpty(category)
            ? _products
            : _products.Where(p => p.Category == category).ToList();

        return View(items);
    }
}