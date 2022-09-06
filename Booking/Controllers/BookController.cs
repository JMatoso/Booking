using AutoMapper;
using Booking.Entities;
using Booking.Extensions;
using Booking.Helpers;
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
        private readonly IMapper _mapper;
        private readonly IBookRepository _books;

        public BookController(IBookRepository books, IHub sentryHub, IMapper mapper)
        {
            _books = books;
            _mapper = mapper;
            _sentryHub = sentryHub;
        }

        /// <summary>
        /// List all books.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BookVM>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookVM>>> GetBooks()
        {
            return Ok(_mapper.Map<List<BookVM>>(await _books.Get()));
        }

        /// <summary>
        /// Search for a book.
        /// </summary>
        [HttpGet("browse")]
        [ProducesResponseType(typeof(List<BookVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BookVM>>> GetBooks([FromQuery]string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return BadRequest(ActionReporterProvider.Set("Empty search is not allowed.", StatusCodes.Status400BadRequest));
            }

            return Ok(_mapper.Map<List<BookVM>>(await _books.Get(search)));
        }

        /// <summary>
        /// Get a book.
        /// </summary>
        [HttpGet("{bookId:Guid}")]
        [ProducesResponseType(typeof(BookVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookVM>> GetBook(Guid bookId)
        {
            if (Guid.Empty == bookId)
            {
                return BadRequest(ActionReporterProvider.Set("Invalid book id.", StatusCodes.Status400BadRequest));
            }

            var book = await _books.Get(bookId);

            return book is null ? 
                NotFound(ActionReporterProvider.Set("Book not found.", StatusCodes.Status404NotFound)) : 
                Ok(_mapper.Map<BookVM>(book));
        }

        /// <summary>
        /// Add a book.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookVM), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBook([FromBody]BookRequest newBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var book = new Book()
            {
                Title = newBook.Title,
                Author = newBook.Author,
                Year = newBook.Year,
                Editor = newBook.Editor,
                Genre = newBook.Genre
            };

            await _books.Add(book);

            return CreatedAtAction(nameof(GetBook), new { bookId = book.Id }, _mapper.Map<BookVM>(book));
        }

        /// <summary>
        /// Update a book.
        /// </summary>
        [HttpPut("{bookId:Guid}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionReporter), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBook(Guid bookId, [FromBody] BookRequest updatedBook)
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

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Year = updatedBook.Year;
            book.Genre = updatedBook.Genre;
            book.Editor = updatedBook.Editor;

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
