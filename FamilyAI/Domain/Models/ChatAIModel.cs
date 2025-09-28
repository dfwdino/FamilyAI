using OllamaSharp.Models.Chat;

namespace FamilyAI.Domain.Models
{
    public class ChatAIModel
    {
        public string BrandUri { get; private set; }
        public string ModelName { get; private set; }
        public PromptType CurrentPromptType { get; set; }
        public ChatRole CurrentChatRole { get; set; }

        public ChatAIModel(string brandUri, string modelName)
        {
            BrandUri = brandUri;
            ModelName = modelName;
        }

    }
}
