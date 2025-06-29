using System.Security.Claims;
using DevLife.Infrastructure.Models.Enums;

namespace DevLife.Features.GenerateExcuses;

public class GenerateExcuse
{
    public static class Endpoint
    {
     
        public record GenerateExcuseCommand(MeetingCategory Category, ExcuseType Type);
        public record GenerateExcuseResponse(string Text, int BelievabilityScore, Guid Id);
        private static readonly Random Rand = new();


        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("excuses/generate", Handler)
                .RequireAuthorization()
                .WithTags("Excuse Generator");
        }
        
        public static IResult Handler(
            GenerateExcuseCommand command,
            ClaimsPrincipal userPrincipal)
        {
            if(userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value is null)
                return Results.Unauthorized();
            
            var key = (command.Category, command.Type);
            if (!Templates.TryGetValue(key, out var options))
                return Results.BadRequest("No excuse templates found for this category and type.");

            var excuse = options[Rand.Next(options.Length)];
            var score = CalculateBelievability(command.Type);

            return Results.Ok(new GenerateExcuseResponse(excuse, score, Guid.NewGuid()));
        }

        private static int CalculateBelievability(ExcuseType type) =>
            type switch
            {
                ExcuseType.Technical => Rand.Next(70, 96),
                ExcuseType.Personal => Rand.Next(50, 81),
                ExcuseType.Creative => Rand.Next(10, 61),
                _ => 0
            };
        
        private static readonly Dictionary<(MeetingCategory, ExcuseType), string[]> Templates = new()
        {
            {
                (MeetingCategory.DailyStandup, ExcuseType.Technical),
                new[] {
                    "CI/CD pipeline broke again.",
                    "Production server spontaneously combusted.",
                    "VS crashed and I forgot my standup password."
                }
            },
            {
                (MeetingCategory.SprintPlanning, ExcuseType.Personal),
                new[] {
                    "My cat walked across the keyboard and pushed everything to main.",
                    "Grandma's Zoom crashed, had to be tech support.",
                    "Emergency therapy session — too much planning stress."
                }
            },
            {
                (MeetingCategory.ClientMeeting, ExcuseType.Creative),
                new[] {
                    "AI gained consciousness and needs emotional support.",
                    "Got invited to join Anonymous mid-call.",
                    "Quantum computer opened a wormhole in the office."
                }
            }
        };
    }
}