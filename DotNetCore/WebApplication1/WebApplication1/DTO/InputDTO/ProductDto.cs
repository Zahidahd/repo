using static WebApplication1.Enums.ProductColours;

namespace WebApplication1.DTO.InputDTO
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string ProductName  { get; set; }

        public string BrandName { get; set; }   

        public ProductColour ProductColour { get; set; }   

        public int Price { get; set; }  

        public int PinCode { get; set; }

        public string LaunchDate { get; set; }
    }
}
