using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class ViewPostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public ViewPostView(
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ViewPost()
    {
        Console.Write("Post Id: ");
        int postId = int.Parse(Console.ReadLine()!);

        Post post = await postRepository.GetSingleAsync(postId);

        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        
        IQueryable<Comment> comments = commentRepository.GetManyAsync();

        foreach (Comment comment in comments)
        {
            if (comment.PostId == postId)
            {
                Console.WriteLine($"Comment: {comment.Body}");
            }
        }
    }
}