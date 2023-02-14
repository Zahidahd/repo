using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        [Route("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            List<OrderDto> orders = _orderRepository.GetAllOrdersAsList();

            if (orders.Count > 0)
                return Ok(orders);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetOrdersCount")]
        public IActionResult GetOrdersCount()
        {
            int orderCount = _orderRepository.GetOrdersCount();
            return Ok(orderCount);
        }

        [HttpGet]
        [Route("GetOrderDetailById/{orderId}")]
        public IActionResult GetOrderDetailById(int orderId)
        {
            if (orderId < 1)
            {
                return BadRequest("OrderId should be greater than 0");
            }

            OrderDto order = _orderRepository.GetOrderDetailById(orderId);

            if (order is not null)
                return Ok(order);
            else
                return NotFound("No Record Found for given id");
        }

        [HttpGet]
        [Route("GetOrdersByCustomerId/{customerId}")]
        public IActionResult GetOrdersByCustomerId(int customerId)
        {
            List<OrderDto> orders = _orderRepository.GetOrdersByCustomerId(customerId);

            if (orders.Count > 0)
                return Ok(orders);
            else
                return NotFound();
        }

        [HttpPost]
        [Route("OrderAdd")]
        public IActionResult OrderAdd([FromBody] OrderDto order)
        {
            try
            {                
                if (ModelState.IsValid)

                    if (ModelState.IsValid)
                    {
                       int orderId = _orderRepository.Add(order);
                        return Ok(orderId);
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
        [Route("OrderUpdate")]
        public IActionResult OrderUpdate([FromBody] OrderDto order)
        {
            try
            {
                string errorMessage = validateOrderAddOrUpdate(order, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);    

                if (ModelState.IsValid)
                {
                    _orderRepository.Update(order);
                    return Ok("Order updated");
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

        private string validateOrderAddOrUpdate(OrderDto order, bool isUpdate = false)
        {
            string errorMessage = "";

            order.ProductName = order.ProductName.Trim();
            var orderDateTime = DateTime.Parse(order.OrderDate);

            if (order.CustomerId < 1)
                errorMessage = "CustomerId Should be greater than 0";
            
            else if(string.IsNullOrWhiteSpace(order.ProductName))  
                errorMessage = "ProductName can not be blank";
             
            else if(order.ProductName.Length < 3 || order.ProductName.Length > 30) 
                errorMessage = "ProductName should be between 3 and 30 characters.";
            
            else if(order.Amount < 50)  
               errorMessage = "Invalid amount, order amount should be above 50";
            
            else if(orderDateTime > DateTime.Now)         
                errorMessage = "Order Date cannot be greater than current date";

            return errorMessage;
        }
    }
}
