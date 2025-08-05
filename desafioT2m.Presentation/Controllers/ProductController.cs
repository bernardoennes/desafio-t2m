using desafioT2m.Dto;
using desafioT2m.Service;
using Microsoft.AspNetCore.Mvc;

namespace desafio_t2m.Controller
{
    [ApiController]
    [Route("estoque/")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
        {
            var products = await _service.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{barCode}")]
        public async Task<ActionResult<ProductDTO>> GetByBarCode(string barCode)
        {
            var product = await _service.GetProductByBarCode(barCode);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductDTO dto)
        {
            var existing = await _service.GetProductByBarCode(dto.BarCode);
            if (existing != null)
                return Conflict("Já existe um produto com esse código de barras.");

            await _service.AddProduct(dto);
            return CreatedAtAction(nameof(GetByBarCode), new { barCode = dto.BarCode }, dto);
        }

        [HttpPut("{barCode}")]
        public async Task<ActionResult> Update(string barCode, [FromBody] ProductDTO dto)
        {
            if (!string.Equals(barCode, dto.BarCode, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _service.GetProductByBarCode(dto.BarCode);
                if (other != null)
                    return Conflict("Já existe outro produto com esse código de barras.");
            }

            var existing = await _service.GetProductByBarCode(barCode);
            if (existing == null)
                return NotFound();

            await _service.UpdateProduct(barCode, dto);
            return NoContent();
        }

        [HttpDelete("{barCode}")]
        public async Task<ActionResult> Delete(string barCode)
        {
            var existing = await _service.GetProductByBarCode(barCode);
            if (existing == null)
                return NotFound();

            await _service.DeleteProduct(barCode);
            return NoContent();
        }
    }
}
