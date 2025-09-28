using FamilyAI.Infrastructure.configuration;

namespace FamilyAI.Domain.Models
{
    public class PromptServiceModel
    {
        public string GetPrompt(PromptType type) => type switch
        {
            PromptType.Educational => PromptTemplates.Educational,
            PromptType.CSharp => PromptTemplates.CSharp,
            PromptType.Direct => PromptTemplates.Direct,
            _ => PromptTemplates.Educational
        };
    }
}
