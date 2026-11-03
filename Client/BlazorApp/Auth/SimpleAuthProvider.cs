
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;

    private ClaimsPrincipal currentClaimsPrincipal = new();

    private string? token;

    public SimpleAuthProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task Login(string userName, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest(userName, password));

        string content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        using JsonDocument document = JsonDocument.Parse(content);

        JsonElement root = document.RootElement;

        token = root.GetProperty("token").GetString();

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(
            root.GetProperty("user").GetRawText(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim("Id", userDto.Id.ToString())
        };

        ClaimsIdentity identity = new(claims, "apiauth");

        currentClaimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(currentClaimsPrincipal)));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(currentClaimsPrincipal));
    }

    public void Logout()
    {
        token = null;

        httpClient.DefaultRequestHeaders.Authorization = null;

        currentClaimsPrincipal = new ClaimsPrincipal();

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(currentClaimsPrincipal)));
    }
}
