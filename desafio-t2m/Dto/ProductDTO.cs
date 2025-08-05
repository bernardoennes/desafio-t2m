using System.Text.Json.Serialization;

namespace desafio_t2m.Dto
{
    public class ProductDTO
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public string Description { get; set; } = "";
        public decimal Price { get; set; }

        [JsonPropertyName("stockLevel")]
        public string StockLevel {
            get {
                if (Quantity < 10)
                    return "Crítico";
                if (Quantity < 100)
                    return "Baixo";
                return "Bom";
            }
        }
    }
}
