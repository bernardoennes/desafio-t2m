using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace desafioT2m.Dto
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "O código de barras é obrigatório.")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "O código de barras deve estar no formato XXXX-XXXX, apenas números.")]
        public string barcode { get; set; } = "";

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(1, ErrorMessage = "O nome não pode ser vazio.")]
        public string name { get; set; } = "";

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        [MinLength(1, ErrorMessage = "A descrição não pode ser vazia.")]
        public string description { get; set; } = "";

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser menor que zero.")]
        public decimal price { get; set; }

        [JsonPropertyName("stockLevel")]
        public string stockLevel
        {
            get
            {
                if (quantity < 10)
                    return "Crítico";
                if (quantity < 100)
                    return "Baixo";
                return "Bom";
            }
        }
    }
}
