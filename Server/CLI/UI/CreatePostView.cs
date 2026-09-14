using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CreatePostView
{
    private readonly IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task CreatePost()
    {
        Console.Write("Title: ");
        string title = Console.ReadLine()!;

        Console.Write("Body: ");
        string body = Console.ReadLine()!;

        Console.Write("User Id: ");
        int userId = int.Parse(Console.ReadLine()!);

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post created = await postRepository.AddAsync(post);

        Console.WriteLine("Post created!");
    }
}