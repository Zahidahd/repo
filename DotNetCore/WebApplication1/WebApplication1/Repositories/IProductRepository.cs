using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IProductRepository
    {
        public List<ProductDto> GetAllProductsAsList();
        public int GetProductsCount();
        public ProductDto GetProductDetailById(int productId);
        public List<ProductDto> GetProductsByBrandNameByPriceUpto(string brandName, int priceUpto);
        public List<ProductDto> GetProductsByPriceRange(int minimumPrice, int maximumPrice);
        public List<ProductDto> GetDeliverableProductsByPincode(int pincode);
        public List<ProductDto> GetProductsDetailByBrandNameByProductName(string brandName, string? productName);
        public string GetProductNameById(int productId);
        public int Add(ProductDto product);
        public void Update(ProductDto product);
    }
}
    