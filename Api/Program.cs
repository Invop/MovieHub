using Asp.Versioning;
using MovieHub.Api.Mapping;
using MovieHub.Application;
using MovieHub.Application.Data;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDatabase(config["Database:ConnectionString"]!);
builder.Services.AddApplication();

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
}).AddMvc().AddApiExplorer();

builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c =>
        c.Cache()
            .Expire(TimeSpan.FromSeconds(30))
            .SetVaryByQuery(["title", "year", "sortBy", "page", "pageSize"])
            .Tag("movies"));
});

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MovieHubDbContext>();

var app = builder.Build();

app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();


app.Run();