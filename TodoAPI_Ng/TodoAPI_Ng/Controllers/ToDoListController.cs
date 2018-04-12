using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI_Ng.Data;
using TodoAPI_Ng.Models;

namespace TodoAPI_Ng.Controllers
{
    [Produces("application/json")]
    [Route("api/ToDoList")]
    public class ToDoListController : Controller
    {
        private readonly ToDoNgDbContext _context;

        public ToDoListController(ToDoNgDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.ToDoList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetToDoList(int id)
        {
            try
            {
                ToDoList list = await _context.ToDoList.FirstAsync(l => l.Id == id);
                IEnumerable<ToDo> items = await _context.ToDo.Where(i => i.ListId == id)
                                                             .ToListAsync();

                // Return an object with the requested list along with its ToDo items
                return Ok(new { list, items });
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ToDoList list)
        {
            if (list is null || !ModelState.IsValid)
            {
                return BadRequest("Empty or invalid ToDoList body provided");
            }

            await _context.ToDoList.AddAsync(list);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // TODO: Insert logging here
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit new ToDoList entity to database");
            }

            return CreatedAtAction("GetToDoList", new { list.Id }, list);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ToDoList list)
        {
            if (list is null || id != list.Id || !ModelState.IsValid)
            {
                return BadRequest("Provided ToDoList body is empty, invalid, or does not match the id provided in routing");
            }

            ToDoList existingList;

            try
            {
                existingList = await _context.ToDoList.FirstAsync(l => l.Id == id);
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }

            // Update the existing list, leaving its Id alone
            existingList.Name = list.Name;
            _context.ToDoList.Update(existingList);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // TODO: Insert logging here
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit update to backend database");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            ToDoList list;

            try
            {
                list = await _context.ToDoList.FirstAsync(l => l.Id == id);
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }

            // Remove all of the items in this list as well
            foreach (ToDo item in _context.ToDo.Where(i => i.ListId == id))
            {
                _context.ToDo.Remove(item);
            }

            _context.ToDoList.Remove(list);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit deletion of list and its items on backend database");
            }

            return NoContent();
        }
    }
}