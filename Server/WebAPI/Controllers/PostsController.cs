
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostsController(
        IPostRepository postRepo,
        IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // CREATE
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request)
    {
        Post post = new()
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };

        Post created = await postRepo.AddAsync(post);

        PostDto dto = new()
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return Created($"/posts/{dto.Id}", dto);
    }

    // GET MANY — optional filters
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p =>
                p.Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> userIds = userRepo.GetManyAsync()
                .Where(u => u.Username.Contains(
                    userName,
                    StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            posts = posts.Where(p => userIds.Contains(p.UserId));
        }

        IEnumerable<PostDto> postDtos = posts.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        });

        return Ok(postDtos);
    }

    // GET SINGLE
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingle(
        [FromRoute] int id)
    {
        Post post = await postRepo.GetSingleAsync(id);

        PostDto dto = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };

        return Ok(dto);
    }

    // UPDATE
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost(
        [FromRoute] int id,
        [FromBody] CreatePostDto request)
    {
        Post post = await postRepo.GetSingleAsync(id);

        post.Title = request.Title;
        post.Body = request.Body;
        post.UserId = request.UserId;

        await postRepo.UpdateAsync(post);

        PostDto dto = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        };

        return Ok(dto);
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(
        [FromRoute] int id)
    {
        await postRepo.DeleteAsync(id);

        return NoContent();
    }
}
