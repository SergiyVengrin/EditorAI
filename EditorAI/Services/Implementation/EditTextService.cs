using EditorAI.Services.Interfaces;

namespace EditorAI.Services.Implementation;

public class EditTextService : IEditTextService
{
    private readonly IHttpClient _httpClient;

    public EditTextService(IHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> EditText(string existingText, string prompt)
    {
        return await _httpClient.EditTextAsync(existingText, prompt);
    }
}