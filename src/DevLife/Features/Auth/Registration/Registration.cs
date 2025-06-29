using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.Auth.Registration;

public class Registration
{
    public record RegistrationRequest(
        string UserName,
        string Name,
        string Surname, 
        DateOnly BirthDate, 
        TechStack TechStack,
        ExperienceLevel ExperienceLevel);
    
    public record RegistrationResponse(string UserName, string FullName, string Stack, string ExperienceLevel);
    
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/registration", Handler)
                .WithTags("Auth");
        }

        private static async Task<IResult> Handler(
            RegistrationRequest request,
            IValidator<RegistrationRequest> validator,
            IPostgresRepository<User> repository,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            
            var isUserExists = await repository
                .Where(u => u.Username == request.UserName)
                .AnyAsync(cancellationToken);
            
            if (isUserExists)
                return Results.BadRequest("User with that username already exists");

            var newUser = User.Create(
                request.UserName,
                request.Name + " " + request.Surname,
                request.BirthDate,
                request.ExperienceLevel,
                request.TechStack);
            
            await repository.AddAsync(newUser, cancellationToken);
            
            return Results.Ok();
        }
    }


    
}