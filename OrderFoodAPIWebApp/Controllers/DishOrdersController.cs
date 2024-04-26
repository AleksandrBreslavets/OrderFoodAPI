using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFoodAPIWebApp.Models;

namespace OrderFoodAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishOrdersController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public DishOrdersController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.DishOrders
                .Include(od=>od.Order)
                    .ThenInclude(r => r.Address)
                    .ThenInclude(a=>a.City)
                .Include(dr => dr.Order)
                    .ThenInclude(r => r.Customer)
                .Include(dr => dr.Dish)
                    .ThenInclude(d => d.Category)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<DishOrder> dishOrder)
        {
            var result = dishOrder.Select(dr => new
            {
                dishQuantity=dr.DishQuantity,
                order = dr.Order != null ? new
                {
                    orderId=dr.OrderId,
                    creationTime=dr.Order.CreationTime,
                    isReady=dr.Order.IsReady,
                    customer = dr.Order.Customer != null ? dr.Order.Customer.CustomerName : null,
                    deliveryAddress = dr.Order.Address != null ? $"{dr.Order.Address.StreetName} {dr.Order.Address.BuldingNumber}" : null,
                    deliveryCity= dr.Order.Address.City != null ? dr.Order.Address.City.Name : null,
                } : null,
                dish = dr.Dish != null ? new
                {
                    dishId = dr.DishId,
                    name = dr.Dish.Name,
                    category = dr.Dish.Category != null ? dr.Dish.Category.Name : null,
                } : null
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

        // GET: api/DishOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishOrder>>> GetDishOrders()
        {
            await MakeIncludes();

            var result = FormResult(await _context.DishOrders.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/DishOrders/5/7
        [HttpGet("{orderId}/{dishId}")]
        public async Task<ActionResult<DishOrder>> GetDishOrder(int orderId, int dishId)
        {
            await MakeIncludes();

            var dishOrder = await _context.DishOrders.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.OrderId == orderId);

            if (dishOrder == null)
            {
                return NotFound(FormRespObject("Немає кортежу з такими ідентифікаторами.", 404));
            }

            var result = FormResult(new List<DishOrder> { dishOrder });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/DishOrders/5/7
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{orderId}/{dishId}")]
        public async Task<IActionResult> PutDishOrder(int orderId, int dishId, DishOrder dishOrder)
        {
            if (orderId != dishOrder.OrderId || dishId!=dishOrder.DishId)
            {
                return BadRequest(FormRespObject("Ідентифікатор замовлення або страви, переданий в URL, не співпадає з ідентифікатором замовлення або страви.", 400));
            }

            if (!DishExists(dishOrder.DishId) || !OrderExists(dishOrder.OrderId))
            {
                return NotFound(FormRespObject("Немає страви або замовлення з такими ідентифікаторами.", 404));
            }

            _context.Entry(dishOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishOrderExists(orderId, dishId))
                {
                    return NotFound(FormRespObject("Немає кортежу з такими ідентифікаторами.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/DishOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DishOrder>> PostDishOrder(DishOrder dishOrder)
        {
            if (!DishExists(dishOrder.DishId) || !OrderExists(dishOrder.OrderId))
            {
                return NotFound(FormRespObject("Немає страви або замовлення з такими ідентифікаторами.", 404));
            }

            _context.DishOrders.Add(dishOrder);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DishOrderExists(dishOrder.OrderId, dishOrder.DishId))
                {
                    return Conflict(FormRespObject("Вже існує кортеж з такими ідентифікаторами замовлення та страви.", 409));
                }
                else
                {
                    throw;
                }
            }

            var res = new
            {
                code = 201,
                data = new
                {
                    dishId = dishOrder.DishId,
                    orderId = dishOrder.OrderId,
                    dishQuantity= dishOrder.DishQuantity
                }
            };

            return CreatedAtAction("GetDishOrder", new { orderId = dishOrder.OrderId, dishId=dishOrder.DishId }, res);
        }

        // DELETE: api/DishOrders/5
        [HttpDelete("{orderId}/{dishId}")]
        public async Task<IActionResult> DeleteDishOrder(int orderId, int dishId)
        {
            var dishOrder = await _context.DishOrders.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.OrderId==orderId);
            if (dishOrder == null)
            {
                return NotFound(FormRespObject("Немає кортежу з таким ідентифікаторами.", 404));
            }

            _context.DishOrders.Remove(dishOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishOrderExists(int orderId, int dishId)
        {
            return _context.DishOrders.Any(e => e.OrderId == orderId && e.DishId==dishId);
        }

        private bool OrderExists(int orderId)
        {
            return _context.Orders.Any(e => e.Id == orderId);
        }

        private bool DishExists(int dishId)
        {
            return _context.Dishes.Any(e => e.Id == dishId);
        }
    }
}
