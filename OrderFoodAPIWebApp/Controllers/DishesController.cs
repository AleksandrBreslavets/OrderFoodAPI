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
    public class DishesController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public DishesController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Dishes
                .Include(a => a.DishRestaurants)
                    .ThenInclude(dr=>dr.Restaurant)
                .Include(a=>a.DishOrders)
                    .ThenInclude(dor=>dor.Order)
                .Include(a=>a.Category)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Dish> dishes)
        {
            var result = dishes.Select(c => new
            {
                dishId = c.Id,
                name = c.Name,
                weight = c.Weight,
                caloriesNumber=c.Calories,
                description = c.Description,
                price =c.Price,
                category=c.Category!=null? c.Category.Name: null,
                restaurants=c.DishRestaurants.Select(dr=> dr.Restaurant!= null? dr.Restaurant.Name:null),
                orders = c.DishOrders.Select(dr => dr.Order != null ? new
                {
                    orderId = dr.Order.Id,
                    creationTime=dr.Order.CreationTime,
                    totalCost=dr.Order.TotalCost
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

        // GET: api/Dishes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Dishes.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            await MakeIncludes();

            var dish = await _context.Dishes.FindAsync(id);

            if (dish == null)
            {
                return NotFound(FormRespObject("Немає страви з такими ідентифікатором.", 404));
            }

            var result = FormResult(new List<Dish> { dish });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Dishes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор страви, переданий в URL, не співпадає з ідентифікатором страви.", 400));
            }

            if (!CategoryExists(dish.CategoryId))
            {
                return NotFound(FormRespObject("Немає категорії з таким ідентифікатором.", 404));
            }

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                {
                    return NotFound(FormRespObject("Немає страви з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Dishes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dish>> PostDish(Dish dish)
        {
            if (!CategoryExists(dish.CategoryId))
            {
                return NotFound(FormRespObject("Немає категорії з таким ідентифікатором.", 404));
            }

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = new
                {
                    dishId = dish.Id,
                    name=dish.Name,
                    weight=dish.Weight,
                    description=dish.Description,
                    price=dish.Price,
                    categoryId=dish.CategoryId,
                    dishRestaurants=dish.DishRestaurants,
                    dishOrders=dish.DishOrders
                }
            };

            return CreatedAtAction("GetDish", new { id = dish.Id }, res);
        }

        // DELETE: api/Dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound(FormRespObject("Немає страви з таким ідентифікатором.", 404));
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Dishes/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetPopularDishes()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Dishes
                .OrderByDescending(d => d.DishOrders.Count)
                .ThenByDescending(d=>d.Price)
                .Take(3) 
                .ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
