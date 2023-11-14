using Core.Entities;

namespace Infrastructure.Specifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecifications<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductSpecification specification)
            : base(x =>
                (string.IsNullOrEmpty(specification.Search) || x.Name.Trim().ToLower().Contains(specification.Search)) &&
                (!specification.BrandId.HasValue || x.ProductBrandId == specification.BrandId) &&
                (!specification.TypeId.HasValue || x.ProductTypeId == specification.TypeId)
            )
        {

        }
    }
}
