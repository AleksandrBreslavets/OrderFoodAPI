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
    public class CitiesController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public CitiesController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Cities
                .Include(c => c.Addresses)
                .ThenInclude(c => c.Restaurants)
                .ToListAsync();     
        }

        private IEnumerable<object> FormResult(List<City> cities)
        {
            var result = cities.Select(c => new
            {
                cityId = c.Id,
                name = c.Name,
                restaurants = c.Addresses.Select(a => a.Restaurants.Select(r => r.Name))
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

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Cities.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            await MakeIncludes();

            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound(FormRespObject("Немає міста з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<City> { city });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор міста, переданий в URL, не співпадає з ідентифікатором міста.", 400));
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound(FormRespObject("Немає міста з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = city
            };

            return CreatedAtAction("GetCity", new { id = city.Id }, res);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound(FormRespObject("Немає міста з таким ідентифікатором.", 404));
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
