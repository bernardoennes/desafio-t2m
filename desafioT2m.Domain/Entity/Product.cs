namespace desafioT2m.Domain;

public class Product
{
    public long Id { get; set; }
    public string BarCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public Product() { }

    public Product(string barCode, string name, int quantity, string description, decimal price)
    {
        BarCode = barCode;
        Name = name;
        Quantity = quantity;
        Description = description;
        Price = price;
    }
}