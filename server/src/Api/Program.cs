using System.Text;
using Api.Apis.CoinMarketCap;
using Api.Common.Attributes;
using Api.Database;
using Api.Swagger;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(DotNetEnv.Env.GetString("DATABASE_CONNECTION"))
);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        string jwtSecret = DotNetEnv.Env.GetString("JWT_SECRET");
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(options =>
    {
        options
            .WithOrigins(DotNetEnv.Env.GetString("ClientAppUrl"))
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddHttpClient<ICoinMarketCapHttpClient, CoinMarketCapHttpClient>(
        (client, serviceProvider) =>
        {
            return new CoinMarketCapHttpClient(client, DotNetEnv.Env.GetString("COIN_MARKET_CAP_API_KEY"));
        }
    )
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.Scan(
    scan =>
        scan.FromCallingAssembly()
            .AddClasses(c => c.WithAttribute<Inject>())
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithScopedLifetime()
            .AddClasses()
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(
    new ExceptionHandlerOptions { ExceptionHandler = new ExceptionHandler(app.Environment).Invoke }
);

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
