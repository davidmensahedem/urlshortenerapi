using UrlShortnerApi.Options;
using UrlShortnerApi.Services.Interfaces;
using UrlShortnerApi.Services.Providers;
using UrlShortnerApi.Storage;

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



app.Run();
