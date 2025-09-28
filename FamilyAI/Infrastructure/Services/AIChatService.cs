using FamilyAI.Domain.Models;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace FamilyAI.Infrastructure.Services
{
    public class AIChatService
    {
        ChatAIModel ChatAISettings = new ChatAIModel("http://localhost:11434/", "llama3.2-vision");
        PromptType SelectedType;
        PromptServiceModel promptService; // Need to DI
        List<ChatMessage> chatHistory;
        IChatClient chatClient; //need to DI
        ChatRole SelectedRole = ChatRole.Assistant;

        public AIChatService()
        {
            // Initialize OllamaApiClient targeting the "gpt-oss:20b" model
            chatClient = new OllamaApiClient(new Uri(ChatAISettings.BrandUri), ChatAISettings.ModelName);

            promptService = new PromptServiceModel();

            //Need to add this to the Chat Model selection. For now, hardcoding to Educational and Assistant.
            SelectedType = PromptTypeExtensions.GetType("kids");
            ChatRole SelectedRole = ChatRole.Assistant;

            chatHistory = new List<ChatMessage>()
                {
                    new ChatMessage(ChatRole.System, promptService.GetPrompt(SelectedType))
                };

        }

        public async Task<string> ChatWithAIAsync(string userInput)
        {


            if (SelectedType.Equals(PromptType.Educational))
            {
                Console.WriteLine("Learning Assistant - Type 'exit' to quit");
                Console.WriteLine("I'm here to help you learn! I'll guide you to find the answers.");
                Console.WriteLine();
            }
            else if (SelectedType.Equals(PromptType.CSharp))
            {
                Console.WriteLine("C# Coding Assistant - Type 'exit' to quit");
                Console.WriteLine("I'm here to help you with C# coding questions and tasks.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Direct Assistant - Type 'exit' to quit");
                Console.WriteLine("I'm here to assist you directly with your questions and tasks.");
                Console.WriteLine();
            }

            while (true)
            {
                Console.Write("You: \n");

                Console.WriteLine();


                string result = CheckInput(userInput);

                if (result == "break")
                {
                    Console.WriteLine("Exiting chat. Goodbye!");
                    chatClient.Dispose();
                    break;
                }
                else if (result == "continue")
                {
                    continue;
                }

                // Add user message to chat history
                chatHistory.Add(new ChatMessage(SelectedRole, userInput));

                var assistantResponse = "";
                using var cts = new CancellationTokenSource();
                var loadingTask = ShowLoadingAnimation(cts.Token);

                try
                {
                    await foreach (var update in chatClient.GetStreamingResponseAsync(chatHistory))
                    {
                        if (!cts.IsCancellationRequested)
                        {
                            cts.Cancel();
                            Console.Write("\r" + new string(' ', 20) + "\r"); // Clear loading animation
                            Console.Write("Assistant: ");
                        }

                        Console.Write(update.Text);
                        assistantResponse += update.Text;
                    }
                }
                finally
                {
                    cts.Cancel();
                    try
                    {
                        await loadingTask;
                    }
                    catch (OperationCanceledException)
                    {

                        // Expected cancellation, ignore
                    }
                }

                // Append assistant message to chat history
                //Need to add only the assistantResponse text and IsReply to DB.
                chatHistory.Add(new ChatMessage(ChatRole.User, assistantResponse));

                return assistantResponse;

            }

            return "";

        }

        string CheckInput(string userInput)
        {
            string ProcessCommand = string.Empty;

            ProcessCommand = CommandChecking(userInput);

            return ProcessCommand;
        }

        string CommandChecking(string userInput)
        {
            string CommandType = string.Empty;

            if (userInput?.ToLower() == "exit")
            {
                CommandType = "break";
            }

            else if (userInput?.Equals("clear", StringComparison.OrdinalIgnoreCase) == true)
            {
                chatHistory.Clear();
                Console.WriteLine("Chat history cleared.");
                Console.WriteLine();

                CommandType = "continue";
            }

            else if (string.IsNullOrWhiteSpace(userInput)) CommandType = "continue";


            return CommandType;
        }


        // Add this method to your class
        static string FormatThinkingMessage(string frame) =>
            $"\r Assistant Thinking{frame}";

        // Then in your ShowLoadingAnimation method:
        async Task ShowLoadingAnimation(CancellationToken cancellationToken)
        {
            string[] frames = { ".", "..", "..,", "....", ".....", "?", "??", "???" };
            int frameIndex = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.Write(FormatThinkingMessage(frames[frameIndex]));
                frameIndex = (frameIndex + 1) % frames.Length;
                await Task.Delay(100, cancellationToken);
            }
        }

    }
}
