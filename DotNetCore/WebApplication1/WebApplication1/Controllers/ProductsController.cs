using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Enums;
using WebApplication1.Repositories;
using static WebApplication1.Enums.GenderTypes;
using static WebApplication1.Enums.ProductColors;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            List<ProductDto> products = _productRepository.GetAllProductsAsList();

            if (products.Count > 0)
                return Ok(products);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetProductsCount")]
        public IActionResult GetProductsCount()
        {
            int productCount = _productRepository.GetProductsCount();
            return Ok(productCount);
        }

        [HttpGet]
        [Route("GetProductDetail/{productId}")]
        public IActionResult GetProductDetailById(int productId)
        {
            if (productId < 1)
                return BadRequest("ProductId should be greater than 0");

            ProductDto product = _productRepository.GetProductDetailById(productId);

            if (product is not null)
                return Ok(product);
            else
                return NotFound("No Record Found for given id");
        }

        [HttpGet]
        [Route("GetProductsByBrandNameByPriceUpto/{brandName}/{priceUpto}")]
        public IActionResult GetProductsByBrandNameByPriceUpto(string brandName, int priceUpto)
        {
            brandName = brandName.Trim();

            if (string.IsNullOrWhiteSpace(brandName))
                return BadRequest("BrandName can not be blank");

            else if (brandName.Length < 3 || brandName.Length > 30)
                return BadRequest("BrandName should be between 3 and 30 characters.");

            else if (priceUpto < 600)
                return BadRequest("priceUpto should be greater than 600");

            List<ProductDto> products = _productRepository.GetProductsByBrandNameByPriceUpto(brandName, priceUpto);

            if (products.Count > 0)
                return Ok(products);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetProductsByPriceRange/{minimumPrice}/{maximumPrice}")]
        public IActionResult GetProductsByPriceRange(int minimumPrice, int maximumPrice)
        {
            if (maximumPrice < minimumPrice)
                return BadRequest("Maximum price cannot be less than minimum price");

            List<ProductDto> products = _productRepository.GetProductsByPriceRange(minimumPrice, maximumPrice);

            if (products.Count > 0)
                return Ok(products);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetDeliverableProductsByPincode/{pincode}")]
        public IActionResult GetDeliverableProductsByPincode(int pincode)
        {
            if (pincode.ToString().Length > 6 || pincode.ToString().Length < 6)
                return BadRequest("Pincode should be of six digits only");

            List<ProductDto> products = _productRepository.GetDeliverableProductsByPincode(pincode);

            if (products.Count > 0)
                return Ok(products);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetProductsDetailByBrandNameByProductName/{brandName}/{productName?}")]
        public IActionResult GetProductsDetailByBrandNameByProductName(string brandName, string? productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest("ProductName can not be blank");

            else if (productName.Length < 3 || productName.Length > 30)
                return BadRequest("ProductName should be between 3 and 30 characters.");

            List<ProductDto> products = _productRepository.GetProductsDetailByBrandNameByProductName(brandName, productName);

            if (products.Count > 0)
                return Ok(products);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetProductNameById/{ProductId}")]
        public IActionResult GetProductNameById(int productId)
        {
            if (productId < 1)
                return BadRequest("product id should be greater than 0");

            string productName = _productRepository.GetProductNameById(productId);
            return Ok(productName);
        }

        [HttpPost]
        [Route("ProductAdd")]
        public IActionResult ProductAdd([FromBody] ProductDto product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int productId = _productRepository.Add(product);
                    return Ok(productId);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("ProductUpdate")]
        public IActionResult ProductUpdate([FromBody] ProductDto product)
        {
            try
            {
                string errorMessage = validateProductAddOrUpdate(product, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    _productRepository.Update(product);
                    return Ok("Product updated");
                }
                return BadRequest("Record not updated");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string validateProductAddOrUpdate(ProductDto product, bool isUpdate = false)
        {
            string errorMessage = "";

            product.ProductName = product.ProductName.Trim();

            if (isUpdate == true)
            {
                if (product.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
                errorMessage = "ProductName can not be blank";

            else if (product.ProductName.Length < 3 || product.ProductName.Length > 30)
                errorMessage = "ProductName should be between 3 and 30 characters.";

            else if (product.Price < 800)
                errorMessage = "Product price should be minimum 800 or more than 800";

            else if (!Enum.IsDefined(typeof(ProductColors), product.ProductColor))
                errorMessage = "Invalid Color";

            return errorMessage;    
        }
    }
}


