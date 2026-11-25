
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;

    public CommentsController(
        ICommentRepository commentRepo,
        IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromBody] CreateCommentDto request)
    {
        Comment comment = new Comment(
            request.Body,
            request.PostId,
            request.UserId);

        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = new()
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId
        };

        return Created($"/comments/{dto.Id}", dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetMany(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Comment> comments = commentRepo.GetManyAsync();

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> userIds = await userRepo.GetManyAsync()
                .Where(u => EF.Functions.Like(
                    u.Username, $"%{userName}%"))
                .Select(u => u.Id)
                .ToListAsync();

            comments = comments.Where(c => userIds.Contains(c.UserId));
        }

        IEnumerable<CommentDto> commentDtos = await comments
            .Select(comment => new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                PostId = comment.PostId,
                UserId = comment.UserId
            })
            .ToListAsync();

        return Ok(commentDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingle(
        [FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);

        CommentDto dto = new()
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(
        [FromRoute] int id,
        [FromBody] CreateCommentDto request)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);

        comment.Body = request.Body;
        comment.PostId = request.PostId;
        comment.UserId = request.UserId;

        await commentRepo.UpdateAsync(comment);

        CommentDto dto = new()
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId
        };

        return Ok(dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] int id)
    {
        await commentRepo.DeleteAsync(id);

        return NoContent();
    }
}
