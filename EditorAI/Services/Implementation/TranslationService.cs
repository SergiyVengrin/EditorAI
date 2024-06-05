using System.Text.Json;
using System.Text.RegularExpressions;
using EditorAI.Models;
using EditorAI.Services.Interfaces;

namespace EditorAI.Services.Implementation;

public class TranslationService : ITranslationService
{
    private readonly IHttpClient _httpClient;

    public TranslationService(IHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TranslationModel>> Translate(Language language)
    {
        IEnumerable<TranslationModel> translationConfigurations =  GetTranslationConfigurations();

        if (language == Language.English)
        {
            return translationConfigurations;
        }

        string? translatedJsonString = await _httpClient.TranslateAsync(translationConfigurations, language);
        
        List<TranslationModel> translations = JsonSerializer.Deserialize<List<TranslationModel>>(ExtractJson(translatedJsonString).Replace("\\n", "").Replace("\\", ""));

        if (translations is not null & translations.Any())
        {
            return translations;
        }

        return translationConfigurations;
    }

    private IEnumerable<TranslationModel> GetTranslationConfigurations()
    {
        string jsonString = File.ReadAllText("translations.json");
        IEnumerable<TranslationModel> translationModels = JsonSerializer.Deserialize<List<TranslationModel>>(jsonString);

        return translationModels;
    }

    private string ExtractJson(string input)
    {
        string pattern = @"\[[^\]]*\]";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(input);

        if (match.Success)
        {
            return match.Value;
        }

        return string.Empty;
    }
}