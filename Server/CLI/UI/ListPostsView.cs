using RepositoryContracts;
using Entities;

namespace CLI.UI;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task ListPosts()
    {
        IQueryable<Post> posts = postRepository.GetManyAsync();

        foreach (Post post in posts)
        {
            Console.WriteLine($"Id: {post.Id}, Title: {post.Title}");
        }
    }
}