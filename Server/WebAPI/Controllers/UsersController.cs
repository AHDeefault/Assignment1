
using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        User user = new User(request.UserName, request.Password);

        User created = await userRepo.AddAsync(user);

        UserDto dto = new()
        {
            Id = created.Id,
            UserName = created.Username
        };

        return Created($"/users/{dto.Id}", dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetMany(
        [FromQuery] string? userName)
    {
        IQueryable<User> users = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(u =>
                EF.Functions.Like(u.Username, $"%{userName}%"));
        }

        List<UserDto> userDtos = await users
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.Username
            })
            .ToListAsync();

        return Ok(userDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(
        [FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);

        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        [FromRoute] int id,
        [FromBody] CreateUserDto request)
    {
        User user = await userRepo.GetSingleAsync(id);

        user.Username = request.UserName;
        user.Password = request.Password;

        await userRepo.UpdateAsync(user);

        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.DeleteAsync(id);

        return NoContent();
    }
}
