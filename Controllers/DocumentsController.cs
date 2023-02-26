using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeCollaborativeApp.Data;
using RealTimeCollaborativeApp.Models;

namespace RealTimeCollaborativeApp.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Documents
        [HttpGet]
        [Route("/[controller]/GetDocuments")]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocuments()
        {
          if (_context.Documents == null)
          {
              return NotFound();
          }
            return await _context.Documents.ToListAsync();
        }
        // GET: api/DocumentUser
        [HttpGet]
        [Route("/[controller]/GetDocumentUser")]
        public async Task<ActionResult<Object>> GetDocumentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var users = await _context.Set<IdentityUser>().ToListAsync();
            var users = await _context.Users.ToListAsync();
            if (users == null)
            {
                return NotFound();
            }
            //we only want the users who are not the logged in ones
            //{ u.Id, u.Username} those are the only fields we want
            return users.Where(u => u.Id != userId).Select(u => new { u.Id, u.UserName }).ToList();
        }
        // GET: api/Documents/5
        [HttpGet("{id}")]
        [Route("/[controller]/GetDocument/{id}")]
        public async Task<ActionResult<Document>> GetDocument(int id)
        {
          if (_context.Documents == null)
          {
              return NotFound();
          }
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

        // PUT: api/Documents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Route("/[controller]/UpdateDocument/{id}")]
        public async Task<IActionResult> PutDocument(int id, Document document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Documents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("/[controller]/CreateDocument")]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
          if (_context.Documents == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Documents'  is null.");
          }
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = document.Id }, document);
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        [Route("/[controller]/DeleteDocument/{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            if (_context.Documents == null)
            {
                return NotFound();
            }
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentExists(int id)
        {
            return (_context.Documents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
