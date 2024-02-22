using LibApp.Data;
using LibApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
     
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public IActionResult GetBooks()
        {
            return Ok(_context.Books.Include(b => b.Genre).ToList());
        }

        // GET: api/books/{id}
        [HttpGet("{id}", Name="GetBook")]
        
        public IActionResult GetBook(int id) 
        {
            var book = _context.Books.Include(b => b.Genre).SingleOrDefault(b => b.Id == id);

            if(book == null)
               return NotFound();
            
            return Ok(book);
        }

  
        // POST: api/books/{id}
        [HttpPost]
        
        public IActionResult CreateBook(Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            _context.Books.Add(book);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        // PUT: api/books/{id}
        [HttpPut("{id}")]
        
        public IActionResult UpdateBook(int id, Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            

            var bookInDb = _context.Books.AsNoTracking().SingleOrDefault(b => b.Id == id);

            if (bookInDb == null)
                return NotFound();

            bookInDb = book;
            _context.Update(book);
            _context.SaveChanges();

            return Ok(bookInDb);
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
      
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.SingleOrDefault(b => b.Id == id);

            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();

            return Ok(book);
        }

        //GET: api/books/generes
        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres);
        }
    }
}
