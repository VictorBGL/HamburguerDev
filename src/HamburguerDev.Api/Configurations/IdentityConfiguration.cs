using HamburguerDev.Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace HamburguerDev.Api.Configurations;

public static class IdentityConfiguration
{
    public static WebApplicationBuilder AddIdentityConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var secret = builder.Configuration["JwtSettings:Segredo"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        if (string.IsNullOrWhiteSpace(secret))
        {
            if (!builder.Environment.IsDevelopment())
            {
                throw new InvalidOperationException("JWT secret não configurado para ambiente não desenvolvimento.");
            }

            var secretFilePath = Path.Combine(builder.Environment.ContentRootPath, "jwt-dev-secret.key");
            if (File.Exists(secretFilePath))
            {
                secret = File.ReadAllText(secretFilePath);
            }
            else
            {
                secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                File.WriteAllText(secretFilePath, secret);
            }
        }

        var issuer = builder.Configuration["JwtSettings:Emissor"];
        var audience = builder.Configuration["JwtSettings:Audiencia"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return builder;
    }
}
