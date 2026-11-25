using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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

    private bool TryGetCurrentUserId(out int userId)
    {
        return int.TryParse(
            User.FindFirst("Id")?.Value,
            out userId);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = new Post(request.Title, request.Body, userId);

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p =>
                EF.Functions.Like(p.Title, $"%{title}%"));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> userIds = await userRepo.GetManyAsync()
                .Where(u => EF.Functions.Like(
                    u.Username, $"%{userName}%"))
                .Select(u => u.Id)
                .ToListAsync();

            posts = posts.Where(p => userIds.Contains(p.UserId));
        }

        List<PostDto> postDtos = await posts.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        }).ToListAsync();

        return Ok(postDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingle(
        [FromRoute] int id,
        [FromQuery] bool includeAuthor,
        [FromQuery] bool includeComments)
    {
        IQueryable<Post> queryForPost = postRepo.GetManyAsync()
            .Where(p => p.Id == id);

        if (includeAuthor)
        {
            queryForPost = queryForPost.Include(p => p.User);
        }

        if (includeComments)
        {
            queryForPost = queryForPost.Include(p => p.Comments);
        }

        PostDto? dto = await queryForPost.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,

            Author = includeAuthor
                ? new UserDto
                {
                    Id = post.User.Id,
                    UserName = post.User.Username
                }
                : null,

            Comments = includeComments
                ? post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    PostId = c.PostId,
                    UserId = c.UserId
                }).ToList()
                : new()
        }).FirstOrDefaultAsync();

        if (dto == null)
            return NotFound();

        return Ok(dto);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost(
        [FromRoute] int id,
        [FromBody] CreatePostDto request)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = await postRepo.GetSingleAsync(id);

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

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(
        [FromRoute] int id)
    {
        if (!TryGetCurrentUserId(out int userId))
            return Unauthorized();

        Post post = await postRepo.GetSingleAsync(id);

        if (post.UserId != userId)
            return Forbid();

        await postRepo.DeleteAsync(id);

        return NoContent();
    }
}