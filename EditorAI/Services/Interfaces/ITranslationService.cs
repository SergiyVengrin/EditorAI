using EditorAI.Models;

namespace EditorAI.Services.Interfaces;

public interface ITranslationService
{
    Task<IEnumerable<TranslationModel>> Translate(Language language);
}