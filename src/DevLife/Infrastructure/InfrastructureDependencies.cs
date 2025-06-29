using System.Text;
using DevLife.Infrastructure.Database.Mongo;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Database.Redis;
using DevLife.Infrastructure.Services.BackgroundServices;
using DevLife.Infrastructure.Services.CodeWars;
using DevLife.Infrastructure.Services.GitHub;
using DevLife.Infrastructure.Services.judge0;
using DevLife.Infrastructure.Services.Jwt;
using DevLife.Infrastructure.Services.OpenAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace DevLife.Infrastructure;

public static class InfrastructureDependencies
{
    public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<ApplicationDbContext>(
                o => o
                    .UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_DATABASE_URL"))
                    .UseSnakeCaseNamingConvention());
        services.Configure<MongoConfiguration>(options =>
        {
            options.ConnectionString = Environment.GetEnvironmentVariable("MONGO_DATABASE_URL");
            options.DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
        });
        services.AddSingleton<MongoContext>();
        
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped(typeof(IPostgresRepository<>), typeof(PostgresRepository<>));
        
        services.Configure<OpenAiConfiguration>(o =>
        {
            o.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            o.Model = Environment.GetEnvironmentVariable("OPENAI_MODEL");
        });

        services.Configure<JwtConfiguration>(o =>
        {
            o.Key = Environment.GetEnvironmentVariable("JWT_SECRET");
            o.Issuer = configuration.GetSection("JWT")["Issuer"];
            o.Audience = configuration.GetSection("JWT")["Audience"];
            o.ExpiresAtMinutes = int.Parse(configuration.GetSection("JWT")["ExpiresAtMinutes"]);
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JWT")["Issuer"],
                    ValidAudience = configuration.GetSection("JWT")["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)),
                    ClockSkew = TimeSpan.Zero
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gamehub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "DevLife API", Version = "v1" });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: \"Bearer abcdef12345\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddHttpClient<CodewarsClient>();

        services.Configure<Judge0Options>(o =>
        {
            o.Key = Environment.GetEnvironmentVariable("JUDGE0_KEY");
        });
        services.AddScoped<Judge0Service>();

        services.Configure<GitHubOAuthOptions>(o =>
        {
            o.ClientId = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID");
            o.ClientSecret = Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");
            o.Redirect = configuration.GetSection("GitHub")["RedirectUrl"];
        });

        services.AddSingleton<DeveloperCardGenerator>();
        
        services.AddSingleton<IGitHubTokenStorage, RedisGitHubTokenStorage>();
        services.AddHttpClient();

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_URL")));

        services.AddHostedService<AutoMatchingService>();
        
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ChatGptService>();
    }
}