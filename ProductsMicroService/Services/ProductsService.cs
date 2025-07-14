using Microsoft.Extensions.Caching.Distributed;
using ProductsMicroService.Contracts;
using ProductsMicroService.Models;
using StackExchange.Redis;

namespace ProductsMicroService.Services
{
    public class ProductsService : IProducts
    {
        private readonly ProductsCatalogContext _context;

        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(ProductsCatalogContext context, IDistributedCache cache, IConnectionMultiplexer redis, ILogger<ProductsService> logger)
        {            
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache;
            _redis = redis;
            _logger = logger;
        }

        public Task<Product> CreateProduct(Product product)
        {
           if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return Task.FromResult(product);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
           if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Product ID invalid.");
            }
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return Task.FromResult(false);
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Product ID invalid.");
            }
            var product = _context.Products.Find(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            return Task.FromResult<IEnumerable<Product>>(_context.Products.ToList());
        }

        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (product.IdProduct <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(product.IdProduct), "Product ID invalid.");
            }
            var existingProduct = _context.Products.Find(product.IdProduct);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.IdProduct} not found.");
            }
            existingProduct.ProductName = product.ProductName;
            existingProduct.Sku = product.Sku;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            _context.Products.Update(existingProduct);
            _context.SaveChanges();
            return Task.FromResult(existingProduct);
        }
    }
}
