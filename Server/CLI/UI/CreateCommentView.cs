using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepository;

    public CreateCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }
    
    public async Task CreateComment()
    {
        Console.Write("Body: ");
        string body = Console.ReadLine()!;

        Console.Write("User Id: ");
        int userId = int.Parse(Console.ReadLine()!);

        Console.Write("Post Id: ");
        int postId = int.Parse(Console.ReadLine()!);

        Comment comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        Comment created = await commentRepository.AddAsync(comment);

        Console.WriteLine("Comment created!");
    }
}