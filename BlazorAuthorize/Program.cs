using BlazorAuthorize;
using BlazorAuthorize.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "HAHA HEHE HOHOHOHOHOHOHO hi github people :wave:";
        var jwtSecretBytes = SHA256.HashData(Encoding.UTF8.GetBytes(jwtSecret));
        var jwtSSK = new SymmetricSecurityKey(jwtSecretBytes);
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = builder.Configuration["Jwt:Issuer"] is not null,
            ValidateAudience = builder.Configuration["Jwt:Audience"] is not null,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = jwtSSK
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorAuthorize.Client._Imports).Assembly);

app.UseAuthentication();
app.UseAuthorization();

app.Run();
