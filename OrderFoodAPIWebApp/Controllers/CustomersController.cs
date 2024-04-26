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
    public class CustomersController : ControllerBase
    {
        private readonly FoodOrderAPIContext _context;

        public CustomersController(FoodOrderAPIContext context)
        {
            _context = context;
        }

        private async Task MakeIncludes()
        {
            await _context.Customers
                .Include(c => c.Orders)
                .ToListAsync();
        }

        private IEnumerable<object> FormResult(List<Customer> customers)
        {
            var result = customers.Select(c => new
            {
                customerId = c.Id,
                name = c.CustomerName,
                phone = c.CustomerPhone,
                orders = c.Orders.Select(o => new
                {
                    orderId=o.Id,
                    creationTime= o.CreationTime,
                    totalCost=o.TotalCost
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

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            await MakeIncludes();

            var result = FormResult(await _context.Customers.ToListAsync());

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            await MakeIncludes();

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(FormRespObject("Немає замовника з таким ідентифікатором.", 404));
            }

            var result = FormResult(new List<Customer> { customer });

            return Ok(new
            {
                code = 200,
                data = result.FirstOrDefault()
            });
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(FormRespObject("Ідентифікатор замовника, переданий в URL, не співпадає з ідентифікатором замовника.", 400));
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound(FormRespObject("Немає замовника з таким ідентифікатором.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormRespObject("Успішно оновлено.", 200));
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customers.Any(c => c.CustomerPhone == customer.CustomerPhone))
            {
                return Conflict(FormRespObject("Замовник з таким номером телефону вже існує.", 409));
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var res = new
            {
                code = 201,
                data = customer
            };

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, res);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(FormRespObject("Немає замовника з таким ідентифікатором.", 404));
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
