using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EditorAI.Models;
using EditorAI.Services.Interfaces;
using Microsoft.AspNetCore.Http;


namespace EditorAI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITranslationService _translationService;
    private readonly IImageService _imageService;
    private readonly IHttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger,
        ITranslationService translationService,
        IHttpClient httpClient, IImageService imageService)
    {
        _logger = logger;
        _translationService = translationService;
        _httpClient = httpClient;
        _imageService = imageService;
    }

    public async Task<IActionResult> Index()
    {
        Language selectedLanguage;
        if (Request.Cookies.TryGetValue("selectedLanguage", out string languageValue))
        {
            Enum.TryParse(languageValue, out selectedLanguage);
            if (Request.Cookies.TryGetValue(selectedLanguage.ToString(), out string languageConfiguratiosJson))
            {
                return View(JsonSerializer.Deserialize<IEnumerable<TranslationModel>>(languageConfiguratiosJson));
            }
        }
        else
        {
            selectedLanguage = Language.English; // Default language
            Response.Cookies.Append("selectedLanguage", Language.English.ToString());
        }

        IEnumerable<TranslationModel> translations = await _translationService.Translate(selectedLanguage);
        var translationsJson = JsonSerializer.Serialize(translations);

        if (selectedLanguage != Language.English)
        {
            Response.Cookies.Append(selectedLanguage.ToString(), translationsJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1)
            });
        }

        return View(translations);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> UploadAudio(IFormFile? audioFile, string? text)
    {
        if (audioFile == null || audioFile.Length == 0)
        {
            return new JsonResult(new { message = "Invalid file" });
        }

        try
        {
            string transciptedText = await _httpClient.UploadAudioAsync(audioFile);
            string response = await _httpClient.EditTextAsync(text ?? string.Empty, transciptedText);

            return new JsonResult(new { responseContent = response });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = $"Error: {ex.Message}" });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> GenerateFromPrompt(string? prompt, string? existingText)
    {
        try
        {
            string response = await _httpClient.EditTextAsync(existingText ?? string.Empty, prompt);
            
            return new JsonResult(new { responseContent = response });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<string> GenerateImage(string prompt)
    {
        if (prompt.Length > 0)
        {
            return await _imageService.GenerateImage(prompt);
        }

        return string.Empty;
    }

    [HttpGet]
    public IActionResult ChangeLanguage(Language language)
    {
        // Store the selected language in a cookie
        Response.Cookies.Append("selectedLanguage", language.ToString());

        return RedirectToAction("Index");
    }
}