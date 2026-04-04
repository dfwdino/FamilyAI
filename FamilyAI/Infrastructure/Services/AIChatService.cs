using FamilyAI.Domain.Models;
using FamilyAI.Infrastructure.configuration;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace FamilyAI.Infrastructure.Services
{
    public class AIChatService
    {
        private readonly ChatAIModel _chatAISettings = new ChatAIModel("http://localhost:11434/", "llama3.2-vision");
        private readonly IChatClient _chatClient;
        private List<ChatMessage> _chatHistory;

        public AIChatService()
        {
            _chatClient = new OllamaApiClient(new Uri(_chatAISettings.BrandUri), _chatAISettings.ModelName);
            // Default to the Educational prompt until Initialize() is called
            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, PromptTemplates.Educational)
            };
        }

        /// <summary>
        /// Sets the system prompt and loads existing conversation history.
        /// Call this before sending any messages when opening a chat.
        /// </summary>
        public void Initialize(string systemPrompt, List<ChatLog> existingLogs)
        {
            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemPrompt)
            };

            foreach (var log in existingLogs)
            {
                var role = log.IsReply ? ChatRole.Assistant : ChatRole.User;
                _chatHistory.Add(new ChatMessage(role, log.Text));
            }
        }

        public async Task<string> ChatWithAIAsync(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return string.Empty;

            _chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

            var responseBuilder = new System.Text.StringBuilder();

            await foreach (var update in _chatClient.GetStreamingResponseAsync(_chatHistory))
            {
                responseBuilder.Append(update.Text);
            }

            var responseText = responseBuilder.ToString();
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, responseText));

            return responseText;
        }
    }
}
