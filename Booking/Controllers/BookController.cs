using Booking.Entities;
using Booking.Extensions;
using Booking.Models;
using Booking.Repository;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace Booking.Controllers
{
    /// <summary>
    /// Booking.
    /// </summary>
    [ApiController]
    [Route("api/book")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class BookController : ControllerBase
    {
        private readonly IHub _sentryHub;
        private readonly IBookRepository _books;

        public BookController(IBookRepository books, IHub sentryHub)
        {
            _books = books;
            _sentryHub = sentryHub;
        }

        /// <summary>
        /// List all books.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return Ok(await _books.Get());
        }

        /// <summary>
        /// Search for a book.
        /// </summary>
        [HttpGet("browse")]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery]string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return BadRequest(ActionReporterProvider.Set("Empty search is not allowed.", StatusCodes.Status400BadRequest));
            }

            return Ok(await _books.Get(search));
        }

        /// <summary>
        /// Get a book.
        /// </summary>
        [HttpGet("{bookId:Guid}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book>> GetBook(Guid bookId)
        {
            if (Guid.Empty == bookId)
            {
                return BadRequest(ActionReporterProvider.Set("Invalid book id.", StatusCodes.Status400BadRequest));
            }

            var book = await _books.Get(bookId);

            return book is null ? NotFound(ActionReporterProvider.Set("Book not found.", StatusCodes.Status404NotFound)) : Ok(book);
        }

        /// <summary>
        /// Add a book.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
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

            return CreatedAtAction(nameof(GetBook), new { bookId = book.Id }, book);
        }

        /// <summary>
        /// Update a book.
        /// </summary>
        [HttpPut("{bookId:Guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBook(Guid bookId, [FromBody] BookVM bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = await _books.Get(bookId);

            if(book is null)
            {
                return NotFound(ActionReporterProvider.Set("Book not found.", StatusCodes.Status404NotFound));
            }

            book.Title = bookModel.Title;
            book.Author = bookModel.Author;
            book.Year = bookModel.Year;
            book.Genre = bookModel.Genre;
            book.Editor = bookModel.Editor;

            await _books.Update(book);

            return Ok();
        }

        /// <summary>
        /// Delete a book.
        /// </summary>
        [HttpDelete("{bookId:Guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            if (Guid.Empty == bookId)
            {
                return BadRequest(ActionReporterProvider.Set("Invalid book id.", StatusCodes.Status400BadRequest));
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
