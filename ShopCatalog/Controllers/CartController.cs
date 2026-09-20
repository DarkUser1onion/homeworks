using Microsoft.AspNetCore.Mvc;
using ShopCatalog.Models;

namespace ShopCatalog.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Корзина";

        var cart = HttpContext.Session.GetString("Cart");
        var ids = string.IsNullOrEmpty(cart)
            ? new List<int>()
            : cart.Split(',').Select(int.Parse).ToList();

        var items = ids
            .GroupBy(id => id)
            .Select(g =>
            {
                var p = ShopData.Products.FirstOrDefault(x => x.Id == g.Key);
                if (p == null) return null;
                return new CartItem
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = g.Count()
                };
            })
            .Where(x => x != null)
            .ToList()!;

        return View(items);
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove("Cart");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Checkout()
    {
        HttpContext.Session.Remove("Cart");
        TempData["OrderSuccess"] = "Заказ успешно оформлен!";
        return RedirectToAction(nameof(ThankYou));
    }

    public IActionResult ThankYou()
    {
        ViewData["Title"] = "Спасибо за заказ";
        return View();
    }

}