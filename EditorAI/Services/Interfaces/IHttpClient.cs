using EditorAI.Models;

namespace EditorAI.Services.Interfaces;

public interface IHttpClient
{
    Task<string> UploadAudioAsync(IFormFile audioFile);

    Task<string> TranslateAsync(IEnumerable<TranslationModel> translationConfigurations, Language language);

    Task<string> EditTextAsync(string existingText, string prompt);

    Task<string> GenerateImage(string prompt);

}