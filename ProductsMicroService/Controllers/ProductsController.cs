using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsMicroService.Contracts;

namespace ProductsMicroService.Controllers
{
   
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        public IProducts _productsService;
       
        public ProductsController(IProducts productsService)
        {
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = await _productsService.GetProductsAsync();
            return Ok(products);
        }
    }
}
