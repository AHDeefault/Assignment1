namespace Entities;

public class User
{
    private User() { }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}