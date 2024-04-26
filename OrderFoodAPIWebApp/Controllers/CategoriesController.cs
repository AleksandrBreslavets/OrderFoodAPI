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
    public class CategoriesController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public CategoriesController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Categories
                .Include(c => c.Dishes)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Category> categories)
        {
            var result = categories.Select(c => new
            {
                categoryId = c.Id,
                name = c.Name,
                dishes = c.Dishes.Select(d => new
                {
                    dishId=d.Id,
                    name=d.Name,
                    weight=d.Weight
                })

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

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Categories.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            await MakeIncludes();

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(FormRespObject("Немає категорії з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<Category> { category });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор категорії, переданий в URL, не співпадає з ідентифікатором категорії.", 400));
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(FormRespObject("Немає категорії з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if(_context.Categories.Any(c=>c.Name==category.Name))
            {
                return Conflict(FormRespObject("Категорія з такою назвою вже існує.", 409));
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = category
            };

            return CreatedAtAction("GetCategory", new { id = category.Id }, res);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(FormRespObject("Немає категорії з таким ідентифікатором.", 404));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
