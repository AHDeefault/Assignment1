
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    // Get the ID of the logged-in user from the JWT token
    private bool TryGetCurrentUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirst("Id")?.Value,
            out userId);
    }

    // CREATE
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = new()
        {
            Title = request.Title,
            Body = request.Body,
            UserId = userId
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

    // GET MANY
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
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost(
        [FromRoute] int id,
        [FromBody] CreatePostDto request)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = await postRepo.GetSingleAsync(id);

        // Only the original owner can edit the post
        if (post.UserId != userId)
            return Forbid();

        post.Title = request.Title;
        post.Body = request.Body;

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
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(
        [FromRoute] int id)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = await postRepo.GetSingleAsync(id);

        // Only the original owner can delete the post
        if (post.UserId != userId)
            return Forbid();

        await postRepo.DeleteAsync(id);

        return NoContent();
    }
}
