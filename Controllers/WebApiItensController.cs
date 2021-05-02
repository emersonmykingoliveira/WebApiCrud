using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCrud.Models;

namespace WebApiCrud.Controllers
{
    [Route("api/WebApiItems")]
    [ApiController]
    public class WebApiItensController : ControllerBase
    {
        private readonly WebApiContext _context;

        public WebApiItensController(WebApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WebApiDTO>>> GetWebApiItems()
        {
            return await _context.WebApiItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WebApiDTO>> GetWebApiItem(long id)
        {
            var webApiItem = await _context.WebApiItems.FindAsync(id);

            if (webApiItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(webApiItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWebApiItem(long id, WebApiDTO webApiItemDTO)
        {
            if (id != webApiItemDTO.Id)
            {
                return BadRequest();
            }

            var webApiItem = await _context.WebApiItems.FindAsync(id);
            if (webApiItem == null)
            {
                return NotFound();
            }

            webApiItem.Name = webApiItemDTO.Name;
            webApiItem.IsComplete = webApiItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!webApiItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WebApiDTO>> CreatewebApiItem(WebApiDTO webApiItemDTO)
        {
            var webApiItem = new WebApiItem
            {
                IsComplete = webApiItemDTO.IsComplete,
                Name = webApiItemDTO.Name
            };

            _context.WebApiItems.Add(webApiItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetWebApiItem),
                new { id = webApiItem.Id },
                ItemToDTO(webApiItem));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletewebApiItem(long id)
        {
            var webApiItem = await _context.WebApiItems.FindAsync(id);

            if (webApiItem == null)
            {
                return NotFound();
            }

            _context.WebApiItems.Remove(webApiItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool webApiItemExists(long id) =>
             _context.WebApiItems.Any(e => e.Id == id);

        private static WebApiDTO ItemToDTO(WebApiItem webApiItem) =>
            new WebApiDTO
            {
                Id = webApiItem.Id,
                Name = webApiItem.Name,
                IsComplete = webApiItem.IsComplete
            };
    }
}
