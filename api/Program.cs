using api.configurations;
using Microsoft.AspNetCore.DataProtection;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);
Env.Load();


builder.WebHost.UseUrls("http://0.0.0.0:8000");

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));


DatabaseConfiguration.ConfigurationMongoDb(builder.Services, builder.Configuration);
ServiceConfiguration.ConfigureServices(builder.Services);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapGet("/", () => "Backend is running!");
app.MapControllers();
app.MapRazorPages();

app.Run();
