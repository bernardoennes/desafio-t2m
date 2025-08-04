using desafio_t2m.Domain;
using desafio_t2m.Service;
using Microsoft.AspNetCore.Mvc;

namespace desafio_t2m.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _service.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<Product>> GetById(long id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Product product)
        {
            await _service.AddProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, Product product)
        {
            if (id != product.Id) return BadRequest("ID do produto incorreto.");

            await _service.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            await _service.DeleteProductAsync(id);
            return NoContent();
        }
    }
}