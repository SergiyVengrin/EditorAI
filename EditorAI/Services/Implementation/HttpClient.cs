using EditorAI.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EditorAI.Models;

namespace EditorAI.Services.Implementation;

public class HttpClient : IHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    private readonly string _audioEndpoint;
    private readonly string _chatEndpoint;
    private readonly string _imageEndpoint;
    
    private readonly string _translatePrompt;
    private readonly string _editTextPrompt;
    
    public HttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        
        _audioEndpoint = configuration["InternalAPI:AudioEndpoint"];
        _chatEndpoint = configuration["InternalAPI:ChatEndpoint"];
        _imageEndpoint = configuration["InternalAPI:ImageEndpoint"];
        
        _translatePrompt = configuration["Prompts:TranslatePrompt"];
        _editTextPrompt = configuration["Prompts:EditTextPrompt"];
    }


    public async Task<string> UploadAudioAsync(IFormFile audioFile)
    {
        var client = _httpClientFactory.CreateClient();

        using (var form = new MultipartFormDataContent())
        {
            using (var fileStream = audioFile.OpenReadStream())
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
                form.Add(fileContent, "audioFile", audioFile.FileName);

                var response = await client.PostAsync($"{_audioEndpoint}/UploadAudio", form);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"Error uploading file: {response.StatusCode}");
                }
            }
        }
    }

    public async Task<string> TranslateAsync(IEnumerable<TranslationModel> translationConfigurations, Language language)
    {
        var client = _httpClientFactory.CreateClient();

        IEnumerable<MessageModel> requestContent = new List<MessageModel>()
        {
            new MessageModel()
            {
                Role = "system",
                Content = string.Format(_translatePrompt, language.GetDescription())
            },
            new MessageModel()
            {
                Role = "user",
                Content = JsonSerializer.Serialize(translationConfigurations)
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync($"{_chatEndpoint}/ChatCompletion", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> EditTextAsync(string existingText, string prompt)
    {
        var client = _httpClientFactory.CreateClient();

        IEnumerable<MessageModel> requestContent = new List<MessageModel>()
        {
            new MessageModel()
            {
                Role = "system",
                Content = _editTextPrompt
            },
            new MessageModel()
            {
                Role = "user",
                Content = $"USERPROMPT: {prompt}"
            },
            new MessageModel()
            {
                Role = "user",
                Content = $"EXISTINGTEXT: {existingText}"
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync($"{_chatEndpoint}/ChatCompletion", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }


    public async Task<string> GenerateImage(string prompt)
    {
        var client = _httpClientFactory.CreateClient();

        StringContent content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync($"{_imageEndpoint}/Generate", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}