namespace Entities;

public class Comment
{
    private Comment() { }

    public Comment(string body, int postId, int userId)
    {
        Body = body;
        PostId = postId;
        UserId = userId;
    }

    public int Id { get; set; }
    public string Body { get; set; } = null!;
    public int PostId { get; set; }
    public int UserId { get; set; }

    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}