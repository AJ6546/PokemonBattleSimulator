using PokemonBattleSimulator.Services;
using PokemonBattleSimulator.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var services = builder.Services;

services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

services.AddTransient<IGetPokemon, GetPokemon>();
services.AddTransient<IGetMoves, GetMoves>();
services.AddTransient<ICachePokemon, CachePokemon>();
services.AddTransient<ICacheMoves, CacheMoves>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pokemon}/{action=Index}/{id?}");

CachePokemon(app);
CacheMoves(app);

app.Run();

static void CachePokemon(WebApplication app)
{
    var getPokemon = app.Services.GetRequiredService<ICachePokemon>();
    getPokemon.PopulateCacheAsync();
}
static void CacheMoves(WebApplication app)
{
    var getMoves = app.Services.GetRequiredService<ICacheMoves>();
    getMoves.PopulateCacheAsync();
}
