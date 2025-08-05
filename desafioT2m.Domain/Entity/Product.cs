namespace desafioT2m.Domain;

public class Product
{
    public long Id { get; set; }
    public string barcode { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int quantity { get; set; }
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; }

    public Product() { }

    public Product(string BarCode, string Name, int Quantity, string Description, decimal Price)
    {
        BarCode = barcode;
        Name = name;
        Quantity = quantity;
        Description = description;
        Price = price;
    }
}