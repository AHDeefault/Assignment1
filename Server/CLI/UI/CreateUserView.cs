using RepositoryContracts;
using Entities;

namespace CLI.UI;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task CreateUser()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine()!;

        Console.Write("Password: ");
        string password = Console.ReadLine()!;

        User user = new User
        {
            Username = username,
            Password = password
        };

        User created = await userRepository.AddAsync(user);
        Console.WriteLine("User created!");
    }
}