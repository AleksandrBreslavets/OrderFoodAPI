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
    public class AddressesController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public AddressesController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Addresses
                .Include(a=> a.City)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Address> addresses)
        {
            var result = addresses.Select(c => new
            {
                addressId = c.Id,
                streetName = c.StreetName,
                buildingNumber = c.BuldingNumber,
                city = c.City != null ? c.City.Name : null
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

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Addresses.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            await MakeIncludes();

            var address = await _context.Address.FindAsync(id);

            if (address == null)
            {
                return NotFound(FormRespObject("Немає адреси з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<Address> { address });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор адреси, переданий в URL, не співпадає з ідентифікатором адреси.", 400));
            }

            if (!CityExists(address.CityId))
            {
                return NotFound(FormRespObject("Немає міста з таким ідентифікатором.", 404));
            }

            if (_context.Addresses.Any(c => c.CityId == address.CityId && c.StreetName == address.StreetName && c.BuldingNumber == address.BuldingNumber && c.Id!=id))
            {
                return Conflict(FormRespObject("Вже існує в цьому місті така адреса.", 409));
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound(FormRespObject("Немає адреси з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {

            if (!CityExists(address.CityId))
            {
                return NotFound(FormRespObject("Немає міста з таким ідентифікатором.", 404));
            }

            if (_context.Addresses.Any(c => c.CityId == address.CityId && c.StreetName==address.StreetName && c.BuldingNumber == address.BuldingNumber))
            {
                return Conflict(FormRespObject("Вже існує в цьому місті така адреса.", 409));
            }

            _context.Address.Add(address);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = new
                {
                    addressId=address.Id,
                    streetName=address.StreetName,
                    buildingNumber=address.BuldingNumber,
                    cityId=address.CityId
                }
            };

            return CreatedAtAction("GetAddress", new { id = address.Id }, res);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound(FormRespObject("Немає адреси з таким ідентифікатором.", 404));
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }

        private bool CityExists(int cityId)
        {
            return _context.Cities.Any(e => e.Id==cityId);
        }
    }
}
