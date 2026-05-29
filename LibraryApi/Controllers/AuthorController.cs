using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController:ControllerBase
    {
        private readonly IAuthorRepository _repository;
        public AuthorController(IAuthorRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<AuthorResponseDto>>> GetAllAuthors([FromQuery]AuthorQueryParameters queryParameters)
        {
            var (authors,totalCount)=await _repository.GetAllAsync(queryParameters);
            var pagedResult = new PagedResult<AuthorResponseDto>
            {
                Page=queryParameters.Page,
                PageSize=queryParameters.PageSize,
                TotalCount=totalCount,                
                Data = authors.Select(a => MaptoResponse(a))
            };
            return Ok(pagedResult);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorResponseDto>>GetAuthorById(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            return Ok(existing);
        }
        [HttpPost]
        public async Task<ActionResult>CreateAuthor(AuthorCreateDto createDto)
        {

            var newAuthor=new Author
            {
                Name = createDto.Name,
                Bio = createDto.Bio
            };
            var added= await _repository.CreateAsync(newAuthor);
            await _repository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthorById), new { id = added.Id },MaptoResponse(added));
        }
        [HttpDelete]
        public async Task<ActionResult<AuthorResponseDto>> DeleteAuthor(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

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
