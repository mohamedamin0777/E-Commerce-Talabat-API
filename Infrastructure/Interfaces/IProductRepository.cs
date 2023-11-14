using Core.Entities;

namespace Infrastructure.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int? id);
        Task<IReadOnlyList<Product>> GetProductsAsync();
        Task<IReadOnlyList<ProductBrand>> GetProductBrandAsync();
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}
