using DevLife.Infrastructure.Models.Enums;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;

namespace DevLife.Infrastructure.Services.OpenAI;

public class ChatGptService
{
    private readonly OpenAIAPI _api;
    private readonly string _model;
    private Conversation conversation;
    
    public ChatGptService(IOptions<OpenAiConfiguration> config)
    {
        var settings = config.Value;
        _api = new OpenAIAPI(new APIAuthentication(settings.ApiKey));
        _model = settings.Model;
    }

    public async Task<string> SendAsync(string prompt,CancellationToken cancellationToken)
    {
        conversation = _api.Chat.CreateConversation();
        
        conversation.AppendSystemMessage("do what i will say");
        
        conversation.AppendUserInput(prompt);

        var response = await conversation.GetResponseFromChatbotAsync();
        return response?.Trim() ?? "NoResponse";
    }
    
    public async Task<string> SendAsCharacterAsync(PersonalityType personality, string userInput)
    {
        var prompt = Prompts.GetPromptForPersonality(personality);

        var chat = _api.Chat.CreateConversation();
        chat.AppendSystemMessage(prompt);
        chat.AppendUserInput(userInput);

        var response = await chat.GetResponseFromChatbotAsync();
        return response ?? "NoResponse";
    }
    
    public async Task<string> GetIncorrectSnippet(TechStack languages, ExperienceLevel level)
    {
        var prompt = Prompts.GetIncorrectCodeSnippetPrompt(languages.ToString(), level.ToString());

        conversation = _api.Chat.CreateConversation();
        
        conversation.AppendSystemMessage("you are code generator");
        
        conversation.AppendUserInput(prompt);

        var response = await conversation.GetResponseFromChatbotAsync();
        return response?.Trim() ?? "NoResponse";
    }
    
    public async Task<string> GetCorrectSnippet()
    {
        var prompt = Prompts.GetCorrectCodeSnippetPrompt();
        
        conversation.AppendSystemMessage("you are code generator");
        
        conversation.AppendUserInput(prompt);

        var response = await conversation.GetResponseFromChatbotAsync();
        return response?.Trim() ?? "NoResponse";
    }

    public async Task<string> GetZodiacSignRecommendation(string zodiacSign)
    {
        var prompt = Prompts.GetZodiacSignRecommendationPrompt(zodiacSign);

        conversation = _api.Chat.CreateConversation();
        
        conversation.AppendSystemMessage("give me recommendation for zodiac sign");
        conversation.AppendUserInput(prompt);
        
        var response = await conversation.GetResponseFromChatbotAsync();
        return response?.Trim() ?? "NoResponse";
    }

    public async Task<string> GetCodeFeedBack(string taskDescription, string code, string output)
    {
        var prompt = Prompts.GetCodeFeedBack(taskDescription, code, output);

        conversation = _api.Chat.CreateConversation();
        
        conversation.AppendSystemMessage("you're sarcastic AI reviewer");
        conversation.AppendUserInput(prompt);
        
        var response = await conversation.GetResponseFromChatbotAsync();
        return response?.Trim() ?? "NoResponse";
    }
    
    public async Task<string> AnalyzeDeveloperTypeJson(string code, string commits)
    {
        conversation = _api.Chat.CreateConversation();

        conversation.AppendSystemMessage("You are a code analysis AI that identifies developer personality types based on code.");

        conversation.AppendUserInput(Prompts.GenerateDeveloperTypePrompt(code, commits));

        var response = await conversation.GetResponseFromChatbotAsync();

        return response?.Trim() ?? "NoResponse";
    }
}