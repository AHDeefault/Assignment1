using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Posts", request);

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<PostDto>(content, options)!;
    }

    public async Task<List<PostDto>> GetPostsAsync()
    {
        return await client.GetFromJsonAsync<List<PostDto>>("Posts")
               ?? new List<PostDto>();
    }

    public async Task<PostDto> GetPostAsync(int id)
    {
        return await client.GetFromJsonAsync<PostDto>($"Posts/{id}")
               ?? throw new Exception("Post not found.");
    }

    public async Task<PostDto> UpdatePostAsync(int id, CreatePostDto request)
    {
        HttpResponseMessage response =
            await client.PutAsJsonAsync($"Posts/{id}", request);

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<PostDto>(content, options)!;
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage response =
            await client.DeleteAsync($"Posts/{id}");

        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new Exception(content);
        }
    }
}