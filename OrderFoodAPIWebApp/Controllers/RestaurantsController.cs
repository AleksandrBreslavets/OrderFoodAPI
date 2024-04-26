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
    public class RestaurantsController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public RestaurantsController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Restaurants
                .Include(a => a.DishRestaurants)
                    .ThenInclude(dr=>dr.Dish)
                .Include(a=>a.Address)
                    .ThenInclude(a=>a.City)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Restaurant> rests)
        {
            var result = rests.Select(c => new
            {
                restaurantId = c.Id,
                name=c.Name,
                rating=c.Rating,
                address = c.Address!=null? $"{c.Address.StreetName} {c.Address.BuldingNumber}": null,
                city=c.Address.City != null? c.Address.City.Name : null,
                dishes = c.DishRestaurants.Select(dr => dr.Dish!=null? new
                {
                    dishId=dr.Dish.Id,
                    name=dr.Dish.Name,
                    weight=dr.Dish.Weight,
                    price=dr.Dish.Price
                }: null).ToArray() 
            }).ToList();

            return result;
        }

        private IEnumerable<object?> FormRestaurantDishes(Restaurant r)
        {
            var result = r.DishRestaurants.Select(dr => dr.Dish != null ? new
            {
                dishId = dr.Dish.Id,
                name = dr.Dish.Name,
                weight = dr.Dish.Weight,
                price = dr.Dish.Price,
                calories = dr.Dish.Calories,
                description = dr.Dish.Description,
            } : null).ToList();

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

        // GET: api/Restaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Restaurants.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            await MakeIncludes();

            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound(FormRespObject("Немає ресторана з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<Restaurant> { restaurant});

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestaurant(int id, Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор ресторану, переданий в URL, не співпадає з ідентифікатором ресторану.", 400));
            }

            if (!AddressExists(restaurant.AddressId))
            {
                return NotFound(FormRespObject("Немає адреси з таким ідентифікатором.", 404));
            }

            if (_context.Restaurants.Any(c => c.AddressId==restaurant.AddressId && c.Name==restaurant.Name && c.Id!=restaurant.Id))
            {
                return Conflict(FormRespObject("Вже існує ресторан з такою назвою за цією адресою.", 409));
            }

            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
                {
                    return NotFound(FormRespObject("Немає ресторану з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant)
        {
            if (!AddressExists(restaurant.AddressId))
            {
                return NotFound(FormRespObject("Немає адреси з таким ідентифікатором.", 404));
            }

            if (_context.Restaurants.Any(c => c.AddressId == restaurant.AddressId && c.Name == restaurant.Name))
            {
                return Conflict(FormRespObject("Вже існує ресторан з такою назвою за цією адресою.", 409));
            }

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = new
                {
                    restaurantId = restaurant.Id,
                    name=restaurant.Name,
                    rating = restaurant.Rating,
                    addressId=restaurant.AddressId,
                    dishRestaurants = restaurant.DishRestaurants
                }
            };

            return CreatedAtAction("GetRestaurant", new { id = restaurant.Id }, res);
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound(FormRespObject("Немає ресторану з таким ідентифікатором.", 404));
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Restaurants/5/dishes
        [HttpGet("{id}/dishes")]
        public async Task<ActionResult<Restaurant>> GetRestaurantDishes(int id)
        {
            await MakeIncludes();

            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound(FormRespObject("Немає ресторана з таким ідентифікатором.", 404));
            }

            var result = FormRestaurantDishes(restaurant);

            return Ok(new
            {
                code = 200,
                data = result 
            });
        }

        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.Id == id);
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }
    }
}
