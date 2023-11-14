using Core.Entities;
using Infrastructure.Specifications;
using Services.Helper;
using Services.Services.ProductService.Dto;

namespace Services.Services.ProductSevice
{
    public interface IProductService
    {
        Task<ProductResultDto> GetProductByIdAsync(int? id);
        Task<Pagination<ProductResultDto>> GetProductsAsync(ProductSpecification specification);
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}
