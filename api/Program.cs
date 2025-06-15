using api.configurations;
using Microsoft.AspNetCore.DataProtection;
using DotNetEnv;
using api.Middlewares;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    var secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOny", policy => policy.RequireRole("admin"));
});
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

app.UseMiddleware<ErrorHandlingMiddleware>();

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
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Backend is running!").AllowAnonymous();
app.MapControllers();
app.MapRazorPages();

app.Run();
