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
    [Route("api/ToDo")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoNgDbContext _context;

        public ToDoController(ToDoNgDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.ToDo);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetToDo(int id)
        {
            try
            {
                return Ok(await _context.ToDo.Include(t => t.List)
                                             .FirstAsync(t => t.Id == id));
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ToDo toDo)
        {
            if (toDo is null || !ModelState.IsValid)
            {
                return BadRequest("Empty or invalid ToDo item body provided");
            }

            await _context.AddAsync(toDo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // TODO: Insert logging here
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit new ToDo entity to database");
            }

            return CreatedAtAction("GetToDo", new { toDo.Id }, toDo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ToDo toDo)
        {
            if (toDo is null || id != toDo.Id || !ModelState.IsValid)
            {
                return BadRequest("Provided ToDo item body is empty, invalid, or does not match the id provided in routing");
            }

            ToDo existingToDo;

            try
            {
                existingToDo = await _context.ToDo.FirstAsync(t => t.Id == id);
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }

            // Update existingToDo object to match the user provided toDo without touching the Id
            existingToDo.Message = toDo.Message;
            existingToDo.IsDone = toDo.IsDone;
            existingToDo.ListId = toDo.ListId;

            // If ListId is not null in the PUT body, make sure that it actually corresponds to an
            // existing ToDoList entity
            if (existingToDo.ListId.HasValue && 
                !(await _context.ToDoList.AnyAsync(l => l.Id == existingToDo.ListId)))
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    "The provided ListId in the request body does not correspond with a ToDoList entity");
            }

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

            // Success!
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            ToDo existingToDo;

            try
            {
                existingToDo = await _context.ToDo.FirstAsync(t => t.Id == id);
            }
            catch
            {
                // TODO: Insert logging here
                return NotFound();
            }

            _context.ToDo.Remove(existingToDo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // TODO: Insert logging here
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not commit ToDo entity deletion on backend database");
            }

            return NoContent();
        }
    }
}