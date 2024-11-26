using Microsoft.AspNetCore.Mvc;
using dotatryhard.Models;

namespace dotatryhard.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    // GET All 
    [HttpGet]
    public IActionResult GetProducts()
    {
      return Ok(GetSampleProducts());
    }

    // GET by Id
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
      return Ok(GetSampleProducts().FirstOrDefault(x => x.Id == id));
    }

    // POST
    [HttpPost]
    public IActionResult CreateProduct([FromBody] Product product)
    {
      // Save product
      return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] Product product)
    {
      // Update product 
      return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
      // Delete product
      return NoContent(); 
    }

    // Sample data
    List<Product> GetSampleProducts()
    {
      return new List<Product>()
      {
        new Product() { Id = 1, Name = "Apple", Price = 1.50M },
        new Product() { Id = 2, Name = "Orange", Price = 2.50M },
      };
    }
  }
}