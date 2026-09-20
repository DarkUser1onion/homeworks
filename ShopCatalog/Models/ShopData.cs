namespace ShopCatalog.Models;

public static class ShopData
{
    public static readonly List<Product> Products = BuildProducts();

    private static List<Product> BuildProducts()
    {
        var list = new List<Product>
        {
            new() { Id = 1, Name = "Наушники Sony WH-1000XM5", Description = "Беспроводные, шумоподавление",
                    Price = 29990m, OldPrice = 34990m, IsHit = true, Category = "Электроника",
                    ImageUrl = "https://picsum.photos/seed/headphones/400/300" },
            new() { Id = 2, Name = "Книга «Чистый код»", Description = "Роберт Мартин, классика",
                    Price = 1890m, IsHit = false, Category = "Книги",
                    ImageUrl = "https://picsum.photos/seed/book/400/300" },
            new() { Id = 3, Name = "Футболка оверсайз", Description = "Хлопок 100%, унисекс",
                    Price = 1490m, OldPrice = 2490m, IsHit = true, Category = "Одежда",
                    ImageUrl = "https://picsum.photos/seed/tshirt/400/300" },
            new() { Id = 4, Name = "Механическая клавиатура", Description = "Переключатели Cherry MX",
                    Price = 7490m, Category = "Электроника", Stock = 0,
                    ImageUrl = "https://picsum.photos/seed/keyboard/400/300" },
            new() { Id = 5, Name = "Кружка «Программист»", Description = "Керамика, 350 мл",
                    Price = 590m, IsHit = false, Category = "Дом",
                    ImageUrl = "https://picsum.photos/seed/mug/400/300" },
            new() { Id = 6, Name = "Рюкзак городской", Description = "Водоотталкивающий, 25 л",
                    Price = 3490m, OldPrice = 4290m, Category = "Одежда",
                    ImageUrl = "https://picsum.photos/seed/backpack/400/300" }
        };

        var names = new[] {
            "Смарт-часы", "Планшет", "Мышь беспроводная", "Монитор 27\"", "Веб-камера",
            "Микрофон USB", "Колонка Bluetooth", "Powerbank", "USB-хаб", "Коврик для мыши",
            "Лампа настольная", "Органайзер", "Термокружка", "Блокнот", "Ручка подарочная",
            "Кофе в зёрнах", "Чайник электрический", "Тостер", "Блендер", "Весы кухонные",
            "Наушники TWS", "Гарнитура", "SSD 1TB", "Внешний диск"
        };
        var cats = new[] { "Электроника", "Дом", "Книги", "Одежда" };

        for (int i = 0; i < names.Length; i++)
        {
            var id = 7 + i;
            list.Add(new Product
            {
                Id = id,
                Name = names[i],
                Description = "Описание товара «" + names[i] + "»",
                Price = 990m + (id * 137) % 25000,
                OldPrice = (id % 4 == 0) ? 990m + (id * 137) % 25000 + 1200m : null,
                IsHit = (id % 7 == 0),
                Stock = (id % 11 == 0) ? 0 : 5,
                Category = cats[id % cats.Length],
                ImageUrl = $"https://picsum.photos/seed/p{id}/400/300"
            });
        }

        return list;
    }
}