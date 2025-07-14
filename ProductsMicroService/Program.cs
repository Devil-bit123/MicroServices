using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using ProductsMicroService.Contracts;
using ProductsMicroService.Services;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// enable for debugging purposes
IdentityModelEventSource.ShowPII = true;

// Add Redis services
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// Add Redis connection multiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));

// Register token revocation service

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    
    options.TokenHandlers.Clear();
    options.TokenHandlers.Add(new JwtSecurityTokenHandler());

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
    };

    // Enhanced diagnostics
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            //Console.WriteLine($"Authentication failed: {context.Exception}");
            //Console.WriteLine($"Exception type: {context.Exception.GetType().FullName}");
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var revocationService = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocation>();

            if (context.SecurityToken is JwtSecurityToken jwtToken)
            {
                var token = jwtToken.RawData;

                if (!string.IsNullOrEmpty(token) && await revocationService.IsTokenRevokedAsync(token))
                {
                    context.Fail("Token has been revoked");
                }

                // Opcional: info adicional
                // Console.WriteLine($"Token Algorithm: {jwtToken.Header.Alg}");
                // Console.WriteLine($"Token Expiry: {jwtToken.ValidTo.ToLocalTime()}");
            }
            else
            {
                context.Fail("Invalid token type");
            }

            await Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            if (!string.IsNullOrEmpty(context.Token))
            {
                //Console.WriteLine($"Received token ({context.Token.Length} chars)");

                // Safe token inspection
                var segments = context.Token.Split('.');
                //Console.WriteLine($"Segment count: {segments.Length}");

                if (segments.Length > 0)
                {
                    try
                    {
                        var header = JwtHeader.Base64UrlDeserialize(segments[0]);
                        //Console.WriteLine($"Header: {header}");
                    }
                    catch
                    {
                        //Console.WriteLine($"Header segment: {segments[0]}");
                    }
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Register the DbContext with dependency injection
builder.Services.AddDbContext<ProductsMicroService.Models.ProductsCatalogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsCatalogContext")));
builder.Services.AddSingleton<ITokenRevocation, RedisTokenRevocationService>();

builder.Services.AddScoped<IProducts, ProductsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.RoutePrefix = string.Empty; // Para que abra en /
    });

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
