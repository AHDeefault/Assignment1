using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Users", request);

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<UserDto>(content, options)!;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await client.GetFromJsonAsync<List<UserDto>>("Users")
               ?? new List<UserDto>();
    }
}