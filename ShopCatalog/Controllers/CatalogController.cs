using Microsoft.AspNetCore.Mvc;
using ShopCatalog.Models;

namespace ShopCatalog.Controllers;

public class CatalogController : Controller
{

    public IActionResult Index(string? category)
    {
        ViewData["Title"] = "Каталог товаров";

        ViewBag.Categories = ShopData.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        ViewBag.SelectedCategory = category;

        const int pageSize = 6;

        var filtered = string.IsNullOrEmpty(category)
        ? ShopData.Products
        : ShopData.Products.Where(p => p.Category == category).ToList();

        var firstPage = filtered.Take(pageSize).ToList();
        ViewBag.TotalCount = filtered.Count;

        return View(firstPage);
    }

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var products = ShopData.Products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLowerInvariant();
            products = products.Where(p =>
                p.Name.ToLower().Contains(q) ||
                p.Description.ToLower().Contains(q));
        }

        return PartialView("_ProductList", products.ToList());
    }

    [HttpPost]
    public IActionResult AddToCart(int id)
    {
        var product = ShopData.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Json(new { success = false, message = "Товар не найден" });
        }

        var cart = HttpContext.Session.GetString("Cart");
        var items = string.IsNullOrEmpty(cart)
            ? new List<int>()
            : cart.Split(',').Select(int.Parse).ToList();

        items.Add(id);
        HttpContext.Session.SetString("Cart", string.Join(',', items));

        return Json(new
        {
            success = true,
            cartCount = items.Count,
            productName = product.Name
        });
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        var cart = HttpContext.Session.GetString("Cart");
        var count = string.IsNullOrEmpty(cart) ? 0 : cart.Split(',').Length;
        return Json(new { count });
    }
    [HttpGet]
    public IActionResult LoadMore(int page = 2)
    {
        const int pageSize = 6;
        var all = ShopData.Products;
        var total = all.Count;

        var skip = pageSize * (page - 1);
        var items = all.Skip(skip).Take(pageSize).ToList();

        if (items.Count == 0)
        {
            return Content(string.Empty);
        }

        return PartialView("_ProductList", items);
    }

}