using desafio_t2m.Dto;
using desafio_t2m.Service;
using Microsoft.AspNetCore.Mvc;

namespace desafio_t2m.Controller {

    [ApiController]
    [Route("estoque/[controller]")]
    public class ProductController : ControllerBase {
        private readonly ProductService _service;

        public ProductController(ProductService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll() {
            var products = await _service.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ProductDTO>> GetByName(string name) {
            var product = await _service.GetProductByNameAsync(name);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductDTO dto) {
            await _service.AddProductAsync(dto);
            return CreatedAtAction(nameof(GetByName), new { name = dto.Name }, dto);
        }

        [HttpPut("{name}")]
        public async Task<ActionResult> Update(string name, [FromBody] ProductDTO dto) {
            await _service.UpdateProductAsync(name, dto);
            return NoContent();
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult> Delete(string name) {
            await _service.DeleteProductAsync(name);
            return NoContent();
        }
    }
}
