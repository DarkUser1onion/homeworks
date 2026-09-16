namespace ShopCatalog.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }        // для скидки
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsHit { get; set; }                // для бейджа «Хит»
    public int Stock { get; set; } = 10;           // остаток на складе
    public string Category { get; set; } = string.Empty; // для среднего уровня

    public int DiscountPercent =>OldPrice.HasValue && OldPrice.Value > 0 ? (int)Math.Round((1 - Price / OldPrice.Value) * 100) : 0;
}