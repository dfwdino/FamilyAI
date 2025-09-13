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

// Register as Transient to avoid disposal issues in Blazor Server
builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
        ServiceLifetime.Transient);

// Add minimal authentication services to satisfy the requirement
builder.Services.AddAuthentication()
    .AddCookie(); // Even though you won't use it

// Your custom provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();


builder.Services.AddScoped<UserServices, UserServices>();
builder.Services.AddScoped<ThreadService, ThreadService>();
builder.Services.AddScoped<ChatLogService, ChatLogService>();


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
