
using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;
using DevLife.Infrastructure.Services.OpenAI;
using MongoDB.Driver.Linq;

namespace DevLife.Features.DatingProfile.SendMessage;

public class SendMessage
{
    public record SendMessageCommand(Guid MatchId, string Text);

    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/chat/send", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
            SendMessageCommand command,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<Message> messageRepository,
            IMongoRepository<Match> matchRepository,
            IMongoRepository<UserProfile> profileRepository,
            ChatGptService chatGptSerivce,
            CancellationToken cancellationToken)
        {
            var userName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName is null)
                return Results.Unauthorized();
            
            var match = matchRepository
                .AsQueryable()
                .FirstOrDefault(m => m.Id == command.MatchId);
            
            if (match is null)
                return Results.NotFound("Match not found");

            var user = await profileRepository
                .Where(u => u.Username == userName)
                .FirstOrDefaultAsync(cancellationToken);
            
            var userMessage = new Message
            {
                MatchId = command.MatchId,
                SenderId = user.Id,
                Text = command.Text
            };

            await messageRepository.AddAsync(userMessage, cancellationToken);

            var otherId = match.Profile1Id == user.Id ? match.Profile2Id : match.Profile1Id;
            var otherProfile = profileRepository
                .AsQueryable()
                .FirstOrDefault(p => p.Id == otherId);

            if (otherProfile is null)
                return Results.NotFound("user does not exist");

            if (otherProfile.Username.StartsWith("AI_"))
            {
                var prompt = $"You're AI person {otherProfile.Username}. Пользователь пишет: {command.Text}";
                var aiReplyText = await chatGptSerivce
                    .SendAsCharacterAsync(otherProfile.PersonalityType ?? PersonalityType.Mentor, command.Text);

                var aiReply = new Message
                {
                    MatchId = command.MatchId,
                    SenderId = otherId,
                    Text = aiReplyText,
                    IsAiGenerated = true
                };

                await messageRepository.AddAsync(aiReply, cancellationToken);
                return Results.Ok(new[] { userMessage, aiReply });
            }
            return Results.Ok(userMessage);    
        }
    }
}