using FamilyAI.Domain.Data;
using FamilyAI.Domain.Models;
using FamilyAI.Infrastructure;
using FamilyAI.Infrastructure.Services;
using FamilyAI.Presentation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// FIX: Ensure you have installed the NuGet package ''
// If not, run: dotnet add package Microsoft.EntityFrameworkCore.SqlServer

builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add minimal authentication services to satisfy the requirement
builder.Services.AddAuthentication()
    .AddCookie(); // Even though you won't use it

// Your custom provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();




builder.Services.AddScoped<UserServcies, UserServcies>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Note: No UseAuthentication() since you're handling it custom
app.UseAuthorization();


app.Run();
