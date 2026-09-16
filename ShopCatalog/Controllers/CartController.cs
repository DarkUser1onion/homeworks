using Microsoft.AspNetCore.Mvc;
using ShopCatalog.Models;

namespace ShopCatalog.Controllers;

public class CartController : Controller
{
    private static readonly List<CartItem> _items = new();

    public IActionResult Index()
    {
        ViewData["Title"] = "Корзина";
        return View(_items);
    }

    [HttpPost]
    public IActionResult Add(int productId, string name, decimal price)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            _items.Add(new CartItem { ProductId = productId, Name = name, Price = price });
        }

        TempData["Added"] = name;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Clear()
    {
        _items.Clear();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Checkout()
    {
        _items.Clear();
        TempData["OrderSuccess"] = "Заказ успешно оформлен!";
        return RedirectToAction(nameof(ThankYou));
    }

    public IActionResult ThankYou()
    {
        ViewData["Title"] = "Спасибо за заказ";
        return View();
    }
}