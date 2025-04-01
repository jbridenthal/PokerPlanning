


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<ITeamService, TeamService>();
builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
builder.Services.AddSingleton<IRedisAuthService, RedisAuthService>();
builder.Services.AddSingleton<IRedisAuthRepository, RedisAuthRepository>();
ConfigurationOptions conf = new ConfigurationOptions
{
    EndPoints = { "localhost:6379" },
    Password = "your_secure_password" //todo move to config, probably pull in through docker-compose file 
};
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(conf));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<PokerPlanning.Services.AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthentication("redisAuth")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("redisAuth", options => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
