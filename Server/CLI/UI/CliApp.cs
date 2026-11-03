using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Welcome to the CLI!");
        Console.WriteLine("1. Create user");
        Console.WriteLine("2. Create post");
        Console.WriteLine("3. Add comment");
        Console.WriteLine("4. View posts");
        Console.WriteLine("5. View post");

        string? choice = Console.ReadLine();
        
        if (choice == "1")
        {
            CreateUserView createUserView = new CreateUserView(userRepository);
            await createUserView.CreateUser();
        }
        else if (choice == "2")
        {
            CreatePostView createPostView = new CreatePostView(postRepository);
            await createPostView.CreatePost();
        }
        else if (choice == "3")
        {
            CreateCommentView createCommentView = new CreateCommentView(commentRepository);
            await createCommentView.CreateComment();
        }
        else if (choice == "4")
        {
            ListPostsView listPostsView = new ListPostsView(postRepository);
            await listPostsView.ListPosts();
        }
        else if (choice == "5")
        {
            ViewPostView viewPostView = new ViewPostView(postRepository, commentRepository);
            await viewPostView.ViewPost();
        }
    }
}