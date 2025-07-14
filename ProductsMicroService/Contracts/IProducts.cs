using ProductsMicroService.Models;

namespace ProductsMicroService.Contracts
{
    public interface IProducts
    {
        public Task<IEnumerable<Product>> GetProductsAsync();
        public Task<Product> GetProductByIdAsync(int id);
        public Task<Product> CreateProduct(Product product);
        public Task<Product> UpdateProductAsync(Product product);
        public Task<bool> DeleteProductAsync(int id);
        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    }
}
