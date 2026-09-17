using Api.Configuration;
using Api.Modules.Customer.Application.Abstractions.Authentication;
using Api.Modules.Customer.Application.Address.Create;
using Api.Modules.Customer.Application.Address.Delete;
using Api.Modules.Customer.Application.Address.GetAll;
using Api.Modules.Customer.Application.Address.SetDefault;
using Api.Modules.Customer.Application.Address.Update;
using Api.Modules.Customer.Application.Auth.ChangePassword;
using Api.Modules.Customer.Application.Auth.Delete;
using Api.Modules.Customer.Application.Auth.ForgotPassword;
using Api.Modules.Customer.Application.Auth.Login;
using Api.Modules.Customer.Application.Auth.Logout;
using Api.Modules.Customer.Application.Auth.Refresh;
using Api.Modules.Customer.Application.Auth.Register;
using Api.Modules.Customer.Application.Auth.ResetPassword;
using Api.Modules.Customer.Application.Email.Change;
using Api.Modules.Customer.Application.Email.Confirm;
using Api.Modules.Customer.Application.Profile;
using Api.Modules.Customer.Domain.Repositories;
using Api.Modules.Customer.Infrastructure.Authentication;
using Api.Modules.Customer.Infrastructure.Persistence;
using Api.Shared.Database;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(
        JwtSettings.SectionName))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(
            settings.SecretKey),
        "JWT SecretKey is required.")
    .Validate(settings => settings.SecretKey.Length >= 32,
        "JWT SecretKey must contain at least 32 characters.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(
            settings.Issuer),
        "JWT Issuer is required.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(
            settings.Audience),
        "JWT Audience is required.")
    .Validate(settings => settings.AccessTokenExpirationMinutes > 0,
        "JWT access token expiration must be positive.")
    .Validate(settings => settings.RefreshTokenExpirationDays > 0,
        "JWT refresh token expiration in days must be positive.")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        options.MapInboundClaims = false;

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

builder.Services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

builder.Services.AddScoped<IDbSession, DbSession>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ILoginRepository, LoginRepository>();

builder.Services.AddScoped<Api.Modules.Customer.Application.Abstractions.Persistence.IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();

builder.Services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();

builder.Services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();

builder.Services.AddScoped<RegisterHandler>();

builder.Services.AddScoped<LoginHandler>();

builder.Services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

builder.Services.AddScoped<RefreshHandler>();

builder.Services.AddScoped<LoginHandler>();

builder.Services.AddScoped<RefreshHandler>();

builder.Services.AddScoped<LogoutHandler>();

builder.Services.AddScoped<GetProfileHandler>();

builder.Services.AddScoped<DeleteHandler>();

builder.Services.AddScoped<ChangePasswordHandler>();

builder.Services.AddScoped<UpdateProfileHandler>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<CreateAddressHandler>();

builder.Services.AddScoped<GetAddressesHandler>();

builder.Services.AddScoped<UpdateAddressHandler>();

builder.Services.AddScoped<SetDefaultAddressHandler>();

builder.Services.AddScoped<DeleteAddressHandler>();

builder.Services.AddSingleton<IAccountTokenGenerator, AccountTokenGenerator>();

builder.Services.AddScoped<IAccountActionRequestRepository, AccountActionRequestRepository>();

builder.Services.AddScoped<ChangeEmailHandler>();

builder.Services.AddScoped<ConfirmEmailChangeHandler>();

builder.Services.AddScoped<ForgotPasswordHandler>();

builder.Services.AddScoped<ResetPasswordHandler>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter the JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para rodar no docker prod
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



