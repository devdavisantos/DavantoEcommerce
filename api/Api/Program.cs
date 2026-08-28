using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Persistence;
using Api.Shared.Database;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Auth.Register;
using Api.Modules.Customer.Infrastructure.Authentication;
using FluentValidation;
using Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Modules.Customer.Application.Auth.Login;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(
        JwtSettings.SectionName))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(
            settings.SecretKey), "JWT SecretKey is required.")
    .Validate(settings => settings.SecretKey.Length >= 32,
        "JWT SecretKey must contain at least 32 characters.")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings.SecretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

builder.Services.AddScoped<IDbSession, DbSession>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ILoginRepository, LoginRepository>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();

builder.Services.AddScoped<RegisterHandler>();

builder.Services.AddScoped<LoginHandler>();

builder.Services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para rodar no docker prod
//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
