using FamilyAI.Domain.Data;
using FamilyAI.Presentation;
using FamilyAI.Domain.Models;
using FamilyAI.Infrastructure;
using FamilyAI.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Transient);

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<ThreadService>();
builder.Services.AddScoped<ChatLogService>();
builder.Services.AddScoped<AIChatService>();
builder.Services.AddScoped<PromptSettingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
