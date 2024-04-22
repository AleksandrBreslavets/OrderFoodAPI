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

        // GET: api/DishOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishOrder>>> GetDishOrders()
        {
            return await _context.DishOrders.ToListAsync();
        }

        // GET: api/DishOrders/5/7
        [HttpGet("{dishId}/{orderId}")]
        public async Task<ActionResult<DishOrder>> GetDishOrder(int dishId, int orderId)
        {
            var dishOrder = await _context.DishOrders.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.OrderId == orderId);

            if (dishOrder == null)
            {
                return NotFound();
            }

            return dishOrder;
        }

        // PUT: api/DishOrders/5/7
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{dishId}/{orderId}")]
        public async Task<IActionResult> PutDishOrder(int dishId, int orderId, DishOrder dishOrder)
        {
            if (orderId != dishOrder.OrderId || dishId!=dishOrder.DishId)
            {
                return BadRequest("Ідентифікатор замовлення або страви, переданий в URL, не співпадає з ідентифікатором замовлення або страви.");
            }

            _context.Entry(dishOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishOrderExists(dishId, orderId))
                {
                    return NotFound("Немає кортежу з такими ідентифікатороми.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DishOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DishOrder>> PostDishOrder(DishOrder dishOrder)
        {
            _context.DishOrders.Add(dishOrder);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DishOrderExists(dishOrder.DishId, dishOrder.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDishOrder", new { id = dishOrder.OrderId }, dishOrder);
        }

        // DELETE: api/DishOrders/5
        [HttpDelete("{dishId}/{orderId}")]
        public async Task<IActionResult> DeleteDishOrder(int dishId, int orderId)
        {
            var dishOrder = await _context.DishOrders.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.OrderId==orderId);
            if (dishOrder == null)
            {
                return NotFound();
            }

            _context.DishOrders.Remove(dishOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishOrderExists(int dishId, int orderId)
        {
            return _context.DishOrders.Any(e => e.OrderId == orderId && e.DishId==dishId);
        }
    }
}
