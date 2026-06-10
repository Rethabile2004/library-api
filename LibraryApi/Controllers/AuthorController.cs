using Asp.Versioning;
using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LibraryApi.Controllers
{
    /// <summary>
    /// Manages authors for both authenticated and unauthenticated users.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _repository;

        public AuthorController(IAuthorRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves a paginated list of authors based on the specified query parameters.
        /// </summary>
        /// <param name="queryParameters">The parameters used to filter and paginate the list of authors.</param>
        /// <returns>A paged result containing the authors and pagination information.</returns>
        /// <response code="200">Returns the paginated list of authors.</response>
        [HttpGet]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(PagedResult<AuthorResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<AuthorResponseDto>>> GetAllAuthors([FromQuery] AuthorQueryParameters queryParameters)
        {
            var (authors, totalCount) = await _repository.GetAllAsync(queryParameters);
            var pagedResult = new PagedResult<AuthorResponseDto>
            {
                Page = queryParameters.Page,
                PageSize = queryParameters.PageSize,
                TotalCount = totalCount,
                Data = authors.Select(a => MaptoResponse(a))
            };
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a single author by ID.
        /// </summary>
        /// <param name="id">The ID of the author.</param>
        /// <returns>The author matching the given ID.</returns>
        /// <response code="200">Returns the author.</response>
        /// <response code="404">Author not found.</response>
        [HttpGet("{id}")]
        [EnableRateLimiting("read")]
        [ProducesResponseType(typeof(AuthorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorResponseDto>> GetAuthorById(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            return Ok(existing);
        }

        /// <summary>
        /// Creates a new author. Requires authentication.
        /// </summary>
        /// <param name="createDto">The author details.</param>
        /// <returns>The newly created author.</returns>
        /// <response code="201">Author created successfully.</response>
        /// <response code="401">Authentication required.</response>
        [Authorize]
        [HttpPost]
        [EnableRateLimiting("write")]
        [ProducesResponseType(typeof(AuthorResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreateAuthor(AuthorCreateDto createDto)
        {
            var newAuthor = new Author
            {
                Name = createDto.Name,
                Bio = createDto.Bio
            };
            var added = await _repository.CreateAsync(newAuthor);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthorById), new { id = added.Id }, MaptoResponse(added));
        }

        /// <summary>
        /// Deletes an author by ID. Requires authentication.
        /// </summary>
        /// <param name="id">The ID of the author to delete.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Author deleted successfully.</response>
        /// <response code="401">Authentication required.</response>
        /// <response code="404">Author not found.</response>
        [Authorize]
        [HttpDelete("{id}")]
        [EnableRateLimiting("write")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _repository.DeleteAsync(existing);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        private AuthorResponseDto MaptoResponse(Author author)
        {
            return new AuthorResponseDto
            {
                Bio = author.Bio,
                Id = author.Id,
                Name = author.Name
            };
        }
    }
}