using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFoodAPIWebApp.Models;

namespace OrderFoodAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public OrdersController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Orders
                .Include(a => a.DishOrders)
                    .ThenInclude(dr => dr.Dish)
                .Include(a => a.Address)
                    .ThenInclude(a => a.City)
                .Include(a=>a.Customer)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Order> orders)
        {
            var result = orders.Select(c =>new
            {
                orderId = c.Id,
                deliveryCost=c.DeliveryCost,
                totalCost=c.TotalCost,
                creationTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", c.CreationTime),
                isReady=c.IsReady,
                customer=c.Customer!=null? c.Customer.CustomerName : null,
                deliveryAddress = c.Address != null ? $"{c.Address.StreetName} {c.Address.BuldingNumber}" : null,
                deliveryCity = c.Address.City != null ? c.Address.City.Name : null,
                dishes = c.DishOrders.Select(dr => dr.Dish != null ? new
                {
                    dishId = dr.Dish.Id,
                    name = dr.Dish.Name,
                    price = dr.Dish.Price
                } : null).ToArray()
            }).ToList();
        
            return result;
        }

        private object FormRespObject(string msg, int code)
        {
            object res = new
            {
                code = code,
                message = msg
            };

            return res;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Orders.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            await MakeIncludes();

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(FormRespObject("Немає замовлення з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<Order> { order });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор замовлення, переданий в URL, не співпадає з ідентифікатором замовлення.", 400));
            }

            if (!CustomerExists(order.CustomerId) || !AddressExists(order.AddressId))
            {
                return NotFound(FormRespObject("Немає замовника або адреси з таким ідентифікатором.", 404));
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound(FormRespObject("Немає замовлення з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (!CustomerExists(order.CustomerId) || !AddressExists(order.AddressId))
            {
                return NotFound(FormRespObject("Немає замовника або адреси з таким ідентифікатором.", 404));
            }

            order.CreationTime = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
           
            var res = new
            {
                code = 201,
                data = new
                {
                    orderId = order.Id,
                    deliveryCost=order.DeliveryCost,
                    totalCost=order.TotalCost,
                    creationTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", order.CreationTime),
                    isReady =order.IsReady,
                    customerId=order.CustomerId,
                    addressId=order.AddressId,
                    dishOrders=order.DishOrders,
                }
            };

            return CreatedAtAction("GetOrder", new { id = order.Id }, res);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(FormRespObject("Немає замовлення з таким ідентифікатором.", 404));
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }
    }
}
