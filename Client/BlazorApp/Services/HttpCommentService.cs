using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> AddCommentAsync(
        CreateCommentDto request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync("Comments", request);

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<CommentDto>(content, options)!;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int postId)
    {
        return await client.GetFromJsonAsync<List<CommentDto>>(
                   $"Comments?postId={postId}")
               ?? new List<CommentDto>();
    }
}