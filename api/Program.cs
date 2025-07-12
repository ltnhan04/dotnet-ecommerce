using api.configurations;
using Microsoft.AspNetCore.DataProtection;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using api.Repositories.Customer;
using api.Services;
using api.Repositories;
using api.Interfaces;
using api.Services.Customer;
using api.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api.Repositories.Admin;
using api.Services.Admin;
using api.Interfaces.Repositories;
using api.Interfaces.Services;
using api.Utils;
using api.Common;

var builder = WebApplication.CreateBuilder(args);
Env.Load();


builder.WebHost.UseUrls("https://localhost:8000");

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Logging.AddConsole();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
    .SetApplicationName("iTribe-API");

DatabaseConfiguration.ConfigurationMongoDb(builder.Services, builder.Configuration);
ServiceConfiguration.ConfigureServices(builder.Services);

builder.Services.AddScoped<iTribeDbContext>();
builder.Services.AddScoped<CloudinaryUtils>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPointService, PointService>();
builder.Services.AddScoped<RevenueService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICProductService, CProductService>();

builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAdminOrderRepository, AdminOrderRepository>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, CustomerReviewRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();
builder.Services.AddScoped<ICProductRepository, CProductRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdminReviewService, AdminReviewService>();
// builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPointRepository, PointRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGranularityHelper, GranularityHelper>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"You're not authenticated\"}");
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Access denied\"}");
        }
    };

});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOny", policy => policy.RequireRole("admin"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:8000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "iTribe API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Backend is running!").AllowAnonymous();

app.MapControllers();

app.MapWhen(context => context.Request.Path.StartsWithSegments("/Admin"), builder =>
{
    builder.UseRouting();
    builder.UseAuthentication();
    builder.UseAuthorization();
    builder.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages().RequireAuthorization(policy => policy.RequireRole("admin"));
    });
});

app.Run();
