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
    public class DishRestaurantsController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public DishRestaurantsController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        // GET: api/DishRestaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishRestaurant>>> GetDishRestaurants()
        {
            return await _context.DishRestaurants.ToListAsync();
        }

        // GET: api/DishRestaurants/5
        [HttpGet("{dishId}/{restaurantId}")]
        public async Task<ActionResult<DishRestaurant>> GetDishRestaurant(int dishId, int restaurantId)
        {
            var dishRestaurant = await _context.DishRestaurants.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.RestaurantId == restaurantId);

            if (dishRestaurant == null)
            {
                return NotFound();
            }

            return dishRestaurant;
        }

        // PUT: api/DishRestaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{dishId}/{restaurantId}")]
        public async Task<IActionResult> PutDishRestaurant(int dishId, int restaurantId, DishRestaurant dishRestaurant)
        {
            if (restaurantId != dishRestaurant.RestaurantId || dishId!=dishRestaurant.DishId)
            {
                return BadRequest("Ідентифікатор ресторану або страви, переданий в URL, не співпадає з ідентифікатором ресторану або страви.");
            }

            _context.Entry(dishRestaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishRestaurantExists(dishId, restaurantId))
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

        // POST: api/DishRestaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DishRestaurant>> PostDishRestaurant(DishRestaurant dishRestaurant)
        {
            _context.DishRestaurants.Add(dishRestaurant);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DishRestaurantExists(dishRestaurant.DishId, dishRestaurant.RestaurantId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDishRestaurant", new { id = dishRestaurant.RestaurantId }, dishRestaurant);
        }

        // DELETE: api/DishRestaurants/5
        [HttpDelete("{dishId}/{restaurantId}")]
        public async Task<IActionResult> DeleteDishRestaurant(int dishId, int restaurantId)
        {
            var dishRestaurant = await _context.DishRestaurants.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.RestaurantId == restaurantId);
            if (dishRestaurant == null)
            {
                return NotFound();
            }

            _context.DishRestaurants.Remove(dishRestaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishRestaurantExists(int dishId, int restaurantId)
        {
            return _context.DishRestaurants.Any(e => e.RestaurantId == restaurantId && e.DishId==dishId);
        }
    }
}
