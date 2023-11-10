using System.Text;
using AuthWebApi.Middlewares;
using Helpers.Implementations;
using Helpers.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Repositories.Data;
using Repositories.DataAccess.Implementations;
using Repositories.DataAccess.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using Shared.Constants;
using Shared.Environment;
using Shared.Settings;
using Validators.Options;
using Validators.Utilities;
using Validators.Validators;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "DevelopmentCorsPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddScoped<ISignUpService, SignUpService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILogoutService, LogoutService>();
builder.Services.AddScoped<ITokenRotationService, TokenRotationService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{EnvironmentProvider.CurrentEnvironment}.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

// Entity Framework
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});

// Identity with custom classes
builder.Services.Configure<CustomIdentityOptions>(options =>
{
    // User settings
    options.User.MinUserNameLength = 3;
    options.User.MaxUserNameLength = 30;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.";

    // Password settings
    options.Password.MinLength = 8;
    options.Password.MaxLength = 64;
    options.Password.RequireAnyLetter = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

builder.Services.AddScoped<CustomIdentityErrorDescriber>();
builder.Services.AddScoped<CustomIdentityOptions>(provider => provider.GetRequiredService<IOptions<CustomIdentityOptions>>().Value);
builder.Services.AddScoped<ErrorCodesCategory>();

builder.Services
    .AddIdentityCore<User>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<CustomIdentityErrorDescriber>();


// Remove the default Identity validators, and replace them with custom ones
builder.Services.RemoveAll<IUserValidator<User>>();
builder.Services.RemoveAll<IPasswordValidator<User>>();

builder.Services.AddScoped<IUserValidator<User>, IdentityUserNameValidator<User>>();
builder.Services.AddScoped<IPasswordValidator<User>, IdentityPasswordValidator<User>>();
builder.Services.AddScoped<IEmailValidator<User>, EmailValidator<User>>();


// Register the configuration instances
builder.Services.Configure<AccessTokenSettings>(builder.Configuration.GetSection("AccessToken"));
builder.Services.Configure<RefreshTokenSettings>(builder.Configuration.GetSection("RefreshToken"));
builder.Services.Configure<EmailEncryptionSettings>(builder.Configuration.GetSection("EmailEncryption"));

// Add HttpContextAccessor to provide access to the current HttpContext within services
builder.Services.AddHttpContextAccessor();

// Register the helper classes
builder.Services.AddScoped<IAccessTokenHelper, AccessTokenHelper>();
builder.Services.AddScoped<IRefreshTokenHelper, RefreshTokenHelper>();
builder.Services.AddScoped<IEmailEncryptionHelper, EmailEncryptionHelper>();
builder.Services.AddScoped<ICookieHelper, CookieHelper>();

// Auth configuration
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = CookieNames.AccessToken;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["AccessToken:Issuer"],
            ValidAudience = builder.Configuration["AccessToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AccessToken:SecretKey"]!)),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(builder.Configuration.GetValue<double>("AccessToken:ClockSkewInSeconds")),
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[CookieNames.AccessToken] ?? context.Request.Headers[CookieNames.AccessToken];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentCorsPolicy");
}

// Apply Migrations.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

// Add middlewares before authentication and authorization. The order in which the middlewares are added is important.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/auth"), appBuilder =>
{
    appBuilder.UseMiddleware<TokenRotationHandlingMiddleware>();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
