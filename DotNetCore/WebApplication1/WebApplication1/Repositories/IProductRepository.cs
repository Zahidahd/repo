using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IProductRepository
    {
        public DataTable GetAllProducts();
        public int GetProductsCount();
        public DataTable GetProductDetailById(int productId);
        public DataTable GetProductsDetailByBrandNameByPriceUpto(string brandName, int priceUpto);
        public DataTable GetProductsByPriceRange(int minimumPrice, int maximumPrice);
        public DataTable GetDeliverableProductsByPincode(int pincode);
        public DataTable GetProductsDetailByBrandNameByProductName(string brandName, string? productName);
        public string GetProductNameById(int productId);
        public int Add(ProductDto product);
        public void Update(ProductDto product);
    }
}
