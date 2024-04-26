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

        private async Task MakeIncludes()
        {
            await _context.DishRestaurants
                .Include(dr => dr.Restaurant)
                    .ThenInclude(r => r.Address)
                .Include(dr => dr.Restaurant)
                    .ThenInclude(r=>r.Address)
                    .ThenInclude(a => a.City)
                .Include(dr => dr.Dish)
                    .ThenInclude(d => d.Category)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<DishRestaurant> dishRestaurant)
        {
            var result = dishRestaurant.Select(dr => new
            {
                isDishAvailable = dr.IsDishAvailable,
                restaurant = dr.Restaurant != null ? new
                {
                    restaurantId = dr.RestaurantId,
                    name = dr.Restaurant.Name,
                    rating = dr.Restaurant.Rating,
                    city = dr.Restaurant.Address.City != null ? dr.Restaurant.Address.City.Name : null,
                    address = dr.Restaurant.Address != null ? $"{dr.Restaurant.Address.StreetName} {dr.Restaurant.Address.BuldingNumber}" : null,
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

        // GET: api/DishRestaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDishRestaurants()
        {
            await MakeIncludes();

            var result = FormResult(await _context.DishRestaurants.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }


        // GET: api/DishRestaurants/5
        [HttpGet("{restaurantId}/{dishId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDishRestaurant(int restaurantId, int dishId)
        {
            await MakeIncludes();

            var dishRestaurant = await _context.DishRestaurants.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.RestaurantId == restaurantId);

            if (dishRestaurant == null)
            {
                return NotFound(FormRespObject("Немає кортежу з такими ідентифікаторами.", 404));           
            }

            var result = FormResult(new List<DishRestaurant> { dishRestaurant });

            return Ok(new
            {
                code=200,
                data= result.FirstOrDefault()
            });
        }

        // PUT: api/DishRestaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{restaurantId}/{dishId}")]
        public async Task<IActionResult> PutDishRestaurant(int restaurantId, int dishId, DishRestaurant dishRestaurant)
        {
            if (restaurantId != dishRestaurant.RestaurantId || dishId!=dishRestaurant.DishId)
            {
                return BadRequest(FormRespObject("Ідентифікатор ресторану або страви, переданий в URL, не співпадає з ідентифікатором ресторану або страви.", 400));
            }

            if (!DishExists(dishRestaurant.DishId) || !RestaurantExists(dishRestaurant.RestaurantId))
            {
                return NotFound(FormRespObject("Немає страви або ресторану з такими ідентифікаторами.", 404));
            }

            _context.Entry(dishRestaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishRestaurantExists(restaurantId, dishId))
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

        // POST: api/DishRestaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DishRestaurant>> PostDishRestaurant(DishRestaurant dishRestaurant)
        {
            if (!DishExists(dishRestaurant.DishId) || !RestaurantExists(dishRestaurant.RestaurantId))
            {
                return NotFound(FormRespObject("Немає страви або ресторану з такими ідентифікаторами.", 404));
            }

            _context.DishRestaurants.Add(dishRestaurant);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DishRestaurantExists(dishRestaurant.RestaurantId, dishRestaurant.DishId))
                {
                    return Conflict(FormRespObject("Вже існує кортеж з такими ідентифікаторами ресторану та страви.", 409));
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
                    dishId=dishRestaurant.DishId,
                    restaurantId=dishRestaurant.RestaurantId,
                    isDishAvailable=dishRestaurant.IsDishAvailable
                }
            }; 

            return CreatedAtAction("GetDishRestaurant", new { restaurantId = dishRestaurant.RestaurantId, dishId=dishRestaurant.DishId }, res);
        }

        // DELETE: api/DishRestaurants/5
        [HttpDelete("{restaurantId}/{dishId}")]
        public async Task<IActionResult> DeleteDishRestaurant(int restaurantId, int dishId)
        {
            var dishRestaurant = await _context.DishRestaurants.FirstOrDefaultAsync(dr => dr.DishId == dishId && dr.RestaurantId == restaurantId);
            if (dishRestaurant == null)
            {
                return NotFound(FormRespObject("Немає кортежу з таким ідентифікаторами.", 404));
            }

            _context.DishRestaurants.Remove(dishRestaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishRestaurantExists(int restaurantId, int dishId)
        {
            return _context.DishRestaurants.Any(e => e.RestaurantId == restaurantId && e.DishId==dishId);
        }

        private bool RestaurantExists(int restaurantId)
        {
            return _context.Restaurants.Any(e => e.Id == restaurantId );
        }

        private bool DishExists(int dishId)
        {
            return _context.Dishes.Any(e => e.Id == dishId);
        }
    }
}
