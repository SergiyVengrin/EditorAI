namespace EditorAI.Services.Interfaces;

public interface IImageService
{
    Task<string> GenerateImage(string prompt);

}