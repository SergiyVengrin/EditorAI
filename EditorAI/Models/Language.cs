using System.ComponentModel;

namespace EditorAI.Models;

public enum Language
{
    [Description("English")]
    English = 1,
    
    [Description("French")]
    French = 2,
    
    [Description("German")]
    German = 3,
    
    [Description("Polish")]
    Polish = 4,
    
    [Description("Italian")]
    Italian = 5,
    
    [Description("Ukrainian")]
    Ukrainian = 6
}