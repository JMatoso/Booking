using Booking.Entities;
using Booking.Models;
using Booking.Repository;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace Booking.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IHub _sentryHub;
        private readonly IBookRepository _books;

        public BookController(IBookRepository books, IHub sentryHub)
        {
            _books = books;
            _sentryHub = sentryHub;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return Ok(await _books.Get());
        }

        [HttpGet("browse")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return BadRequest("Empty search is not allowed.");
            }

            return Ok(await _books.Get(search));
        }

        [HttpGet("{bookId:int}")]
        public async Task<ActionResult<Book>> GetBook(int bookId)
        {
            if (bookId < 1)
            {
                return BadRequest("Invalid id.");
            }

            var book = await _books.Get(bookId);

            return book is null ? NotFound("Book not found.") : Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody]BookVM bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = new Book()
            {
                Title = bookModel.Title,
                Author = bookModel.Author,
                Year = bookModel.Year,
                Editor = bookModel.Editor,
                Genre = bookModel.Genre
            };

            await _books.Add(book);

            return Created($"/book/{book.Id}", book);
        }

        [HttpPut("{bookId:int}")]
        public async Task<IActionResult> UpdateBook(int bookId, [FromBody] BookVM bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = await _books.Get(bookId);

            if(book is null)
            {
                return NotFound("Book not found.");
            }

            book.Title = bookModel.Title;
            book.Author = bookModel.Author;
            book.Year = bookModel.Year;
            book.Genre = bookModel.Genre;
            book.Editor = bookModel.Editor;

            await _books.Update(book);

            return Ok();
        }

        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            if (bookId < 1)
            {
                return BadRequest("Invalid id.");
            }

            await _books.Delete(bookId);

            return NoContent();
        }

        [HttpGet("sentry-test")]
        public IActionResult SentryTest()
        {
            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");

            try
            {
                var someValue = new Random().Next(2);

                if(someValue == 0)
                {
                    childSpan?.Finish(SpanStatus.Ok);
                    return Ok();
                }

                throw new Exception();
            } 
            catch(Exception e)
            {
                SentrySdk.CaptureMessage(e.Message);
                childSpan?.Finish(e);
                throw;
            }
        }

        [HttpGet("exception")]
        public IActionResult Exception()
        {
            var childSpan = _sentryHub.GetSpan()?.StartChild("exception-test");

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                childSpan?.Finish(e);
                throw;
            }
        }
    }
}
