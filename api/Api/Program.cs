using Amazon.S3;
using Api.Configuration;
using Api.Integrations.Cloudflare.R2;
using Api.Modules.Admin.Application.Abstractions.Authentication;
using Api.Modules.Admin.Application.Auth.Login;
using Api.Modules.Admin.Application.Auth.Logout;
using Api.Modules.Admin.Application.Auth.Refresh;
using Api.Modules.Admin.Application.Profile;
using Api.Modules.Admin.Domain.Repositories;
using Api.Modules.Admin.Infrastructure.Authentication;
using Api.Modules.Admin.Infrastructure.Persistence;
using Api.Modules.Catalog.Application.Products.Create;
using Api.Modules.Catalog.Application.Abstractions.Persistence;
using Api.Modules.Catalog.Infrastructure.Persistence;
using Api.Modules.Catalog.Application.Abstractions.Storage;
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
using Api.Modules.Catalog.Application.Abstractions.Images;
using Api.Modules.Catalog.Infrastructure.Images;
using Api.Shared.Database;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Settings

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.SecretKey),
        "JWT SecretKey is required.")
    .Validate(
        settings => settings.SecretKey.Length >= 32,
        "JWT SecretKey must contain at least 32 characters.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Issuer),
        "JWT Issuer is required.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Audience),
        "JWT Audience is required.")
    .Validate(
        settings => settings.AccessTokenExpirationMinutes > 0,
        "JWT access token expiration must be positive.")
    .Validate(
        settings => settings.RefreshTokenExpirationDays > 0,
        "JWT refresh token expiration in days must be positive.")
    .ValidateOnStart();

builder.Services.AddOptions<AdminJwtSettings>()
    .Bind(builder.Configuration.GetSection(AdminJwtSettings.SectionName))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Issuer),
        "Admin JWT Issuer is required.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Audience),
        "Admin JWT Audience is required.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.SecretKey),
        "Admin JWT SecretKey is required.")
    .Validate(
        settings => settings.SecretKey.Length >= 32,
        "Admin JWT SecretKey must contain at least 32 characters.")
    .Validate(
        settings => settings.AccessTokenExpirationMinutes > 0,
        "Admin JWT access token expiration must be greater than zero.")
    .Validate(
        settings => settings.RefreshTokenExpirationDays > 0,
        "Admin refresh token expiration must be greater than zero.")
    .ValidateOnStart();

// Authentication

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
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddJwtBearer(AdminAuthenticationSchemes.Admin, options =>
    {
        var adminJwtSettings = builder.Configuration
            .GetSection(AdminJwtSettings.SectionName)
            .Get<AdminJwtSettings>()!;

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(adminJwtSettings.SecretKey)),

            ValidateIssuer = true,
            ValidIssuer = adminJwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = adminJwtSettings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.AddAuthenticationSchemes(
            AdminAuthenticationSchemes.Admin);

        policy.RequireAuthenticatedUser();

        policy.RequireClaim(
            "user_type",
            "ADMIN");
    });
});

// Shared

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<IDbSession, DbSession>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddSingleton<IAccountTokenGenerator, AccountTokenGenerator>();

// Customer - Authentication

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<Api.Modules.Customer.Application.Abstractions.Persistence.IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();
builder.Services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();

builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<DeleteHandler>();
builder.Services.AddScoped<ChangePasswordHandler>();
builder.Services.AddScoped<ForgotPasswordHandler>();
builder.Services.AddScoped<ResetPasswordHandler>();

// Customer - Profile

builder.Services.AddScoped<GetProfileHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();

// Customer - Address

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<CreateAddressHandler>();
builder.Services.AddScoped<GetAddressesHandler>();
builder.Services.AddScoped<UpdateAddressHandler>();
builder.Services.AddScoped<SetDefaultAddressHandler>();
builder.Services.AddScoped<DeleteAddressHandler>();

// Customer - Account Actions

builder.Services.AddScoped<IAccountActionRequestRepository, AccountActionRequestRepository>();

builder.Services.AddScoped<ChangeEmailHandler>();
builder.Services.AddScoped<ConfirmEmailChangeHandler>();

// Admin - Authentication

builder.Services.AddScoped<IAdminJwtService, AdminJwtService>();

builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminRefreshTokenRepository, AdminRefreshTokenRepository>();

builder.Services.AddScoped<IValidator<AdminLoginCommand>, AdminLoginValidator>();

builder.Services.AddScoped<AdminLoginHandler>();
builder.Services.AddScoped<AdminRefreshHandler>();
builder.Services.AddScoped<AdminLogoutHandler>();
builder.Services.AddScoped<GetAdminProfileHandler>();

// Integration R2 Cloudflare
builder.Services.AddOptions<R2Settings>()
    .Bind(builder.Configuration.GetSection(R2Settings.SectionName))
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.AccountId),
        "R2 AccountId is required.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.AccessKeyId),
        "R2 AccessKeyId is required.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretAccessKey),
        "R2 SecretAccessKey is required.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.BucketName),
        "R2 BucketName is required.")
    .ValidateOnStart();

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<R2Settings>>().Value;

    var config = new AmazonS3Config
    {
        ServiceURL = $"https://{settings.AccountId}.r2.cloudflarestorage.com",
        AuthenticationRegion = "auto",
        ForcePathStyle = true
    };

    return new AmazonS3Client(
        settings.AccessKeyId,
        settings.SecretAccessKey,
        config);
});

builder.Services.AddScoped<IObjectStorage, CloudflareR2ObjectStorage>();

// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<IValidator<CreateProductCommand>, CreateProductValidator>();
builder.Services.AddScoped<IProductImageProcessor, ProductImageProcessor>();

// Controllers / OpenAPI

builder.Services.AddControllers();
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

// HTTP Pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Docker production
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();