using DashboardAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashboardAdmin.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardRepository _repository;

    public DashboardController(IDashboardRepository repository) => _repository = repository;

    public IActionResult Index()
    {
        ViewData["ReportPeriod"] = "Сентябрь 2026";
        ViewBag.Currency = "RUB";
        
        return View(_repository.GetAllCards());
    }

    public IActionResult Print()
    {
        ViewData["Mode"] = "Print";
        ViewData["ReportPeriod"] = "Сентябрь 2026";
        ViewBag.Currency = "RUB";
        
        return View("Index", _repository.GetAllCards());
    }
}