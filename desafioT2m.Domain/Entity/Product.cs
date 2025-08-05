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

    public Product(string barcode, string name, int quantity, string description, decimal price)
    {
        this.barcode = barcode;
        this.name = name;
        this.quantity = quantity;
        this.description = description;
        this.price = price;
    }
}