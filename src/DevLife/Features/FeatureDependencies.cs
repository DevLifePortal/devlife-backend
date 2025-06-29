using DevLife.Features.Auth.Authorization;
using DevLife.Features.Auth.GitHub;
using DevLife.Features.Auth.Registration;
using DevLife.Features.BugChaseGame;
using DevLife.Features.Casino;
using DevLife.Features.CodeRoast;
using DevLife.Features.DatingProfile.AutoMatch;
using DevLife.Features.DatingProfile.GetMatches;
using DevLife.Features.DatingProfile.GetMessages;
using DevLife.Features.DatingProfile.GetProfile;
using DevLife.Features.DatingProfile.LikeProfile;
using DevLife.Features.DatingProfile.Setup;
using DevLife.Features.GenerateExcuses;
using DevLife.Features.RepositoryAnalyze;
using FluentValidation;

namespace DevLife.Features;

public static class FeatureDependencies
{
    public static void AddFeaturesDependencies(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegistrationRequestValidator>(includeInternalTypes: true);
        
        services.AddSingleton<GameSessionManager>();
        services.AddSingleton<LeaderboardService>();
        services.AddSignalR(o =>
        {
            o.EnableDetailedErrors = true;
        });
    }

    public static void AddFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
            CreateSession.Endpoint.Map(app);
            ChooseSnippet.Endpoint.Map(app);
            
            Registration.Endpoint.Map(app);
            Authorization.Endpoint.Map(app);
            GitHubAuth.Endpoints.Map(app);
            
            DailyChallenge.Endpoint.Map(app);
            LeaderBoard.Endpoint.Map(app);
            GetCodeExercise.Endpoint.Map(app);
            SubmitSolution.Endpoint.Map(app);
            
            GetGithubRepos.Endpoint.Map(app);
            GetRepositoryAnalyze.Endpoint.Map(app);
            GetDeveloperCard.Endpoint.Map(app);
            
            AutoMatch.Endpoint.Map(app);
            GetMatchCandidates.Endpoint.Map(app);
            GetMessages.Endpoint.Map(app);
            GetProfile.Endpoint.Map(app);
            LikeProfile.Endpoint.Map(app);
            ProfileRegister.Endpoint.Map(app);
            
            GenerateExcuse.Endpoint.Map(app);
            AddToFavorite.Endpoint.Map(app);
            GetFavorites.Endpoint.Map(app);
            DeleteFavorite.Endpoint.Map(app);
        
        app.MapHub<GameHub>("/gamehub");
    }
}