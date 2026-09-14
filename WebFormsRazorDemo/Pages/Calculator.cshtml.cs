using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CalculatorModel : PageModel
{
    [BindProperty] public double A { get; set; }
    [BindProperty] public double B { get; set; }
    [BindProperty] public string Operation { get; set; } = "+";
    public string? Result { get; set; }

    public void OnPost()
    {
        try
        {
            double r = Operation switch
            {
                "+" => A + B,
                "-" => A - B,
                "*" => A * B,
                "/" => B == 0
                    ? throw new DivideByZeroException("Деление на ноль!")
                    : A / B,
                _   => throw new InvalidOperationException("Неизвестная операция")
            };
            Result = $"Результат: {r}";
        }
        catch (DivideByZeroException ex)
        {
            Result = $"Ошибка: {ex.Message}";
        }
        catch (Exception ex)
        {
            Result = $"Ошибка: {ex.Message}";
        }
    }
}