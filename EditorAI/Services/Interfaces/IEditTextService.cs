namespace EditorAI.Services.Interfaces;

public interface IEditTextService
{
    Task<string> EditText(string existingText, string prompt);
}