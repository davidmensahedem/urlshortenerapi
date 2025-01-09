var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.Configure<UrlShortingConfig>(urlConfig => builder.Configuration.GetSection(nameof(UrlShortingConfig)).Bind(urlConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("api/create-short-url", async (
    [FromBody] ShortenUrlRequest request,
    [FromServices] IUrlShortenerService urlShortenerService,
    HttpContext httpContext
    ) =>
{
    var response = await urlShortenerService.GenerateShortUrl(request.Url!, httpContext.Request);

    httpContext.Response.StatusCode = Convert.ToInt32(response.Code);

    await httpContext.Response.WriteAsJsonAsync(response);
});


app.MapGet("api/{code}", async (
    string code,
    [FromServices] IUrlShortenerService urlShortenerService,
    HttpContext httpContext
    ) =>
{
    var response = await urlShortenerService.RedirectUrl(code);

    httpContext.Response.StatusCode = Convert.ToInt32(response.Code);

    if (httpContext.Response.StatusCode != 200)
        await httpContext.Response.WriteAsJsonAsync(response);

    return Results.Redirect(response.Data!);
}).ExcludeFromDescription();

// check for any pending migrations and run
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var services = scope.ServiceProvider;
    try
    {
        var storageContext = services.GetRequiredService<ApplicationDbContext>();

        var count = (await storageContext.Database.GetPendingMigrationsAsync()).Count();

        if (count > 0)
        {
            logger.LogInformation("found {Count} pending migrations to apply. will proceed to apply them", count);

            await storageContext.Database.MigrateAsync();

            logger.LogInformation("done applying pending migrations");
        }
        else
        {
            logger.LogInformation("no pending migrations found! :)");
        }

    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while performing migration.");
    }
}



await app.RunAsync();
