using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDto> AddPostAsync(CreatePostDto request);
    Task<List<PostDto>> GetPostsAsync();
    Task<PostDto> GetPostAsync(int id);
    Task<PostDto> UpdatePostAsync(int id, CreatePostDto request);
    Task DeletePostAsync(int id);
}