using EditorAI.Services.Interfaces;

namespace EditorAI.Services.Implementation;

public class ImageService : IImageService
{
    private readonly IHttpClient _httpClient;

    public ImageService(IHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateImage(string prompt)
    {
        return await _httpClient.GenerateImage(prompt);
    }
}