using WebApplication1.Enums;
using static WebApplication1.Enums.ProductColors;

namespace WebApplication1.DTO.InputDTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public ProductColors ProductColor { get; set; }
        public int Price { get; set; }
        public int PinCode { get; set; }
        public string LaunchDate { get; set; }
    }
}
