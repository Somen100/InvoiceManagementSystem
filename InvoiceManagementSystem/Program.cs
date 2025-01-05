using InvoiceMgmt.API;
using InvoiceMgmt.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.Extensions.Hosting;
using InvoiceMgmt.API.Middleware;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtKey = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];
var expirationInMinutes = builder.Configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log or check the error details here
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Host.UseSerilog();
var _configuration = builder.Configuration;
builder.Services.RegisterRepos(_configuration);
// Add services to the container.
// Add Stripe configuration
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InvoiceDbContext>(db => db.UseSqlServer(
    builder.Configuration.GetConnectionString("DbConnection"))
);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("logs/application-log.txt", rollingInterval: RollingInterval.Day)
        .ReadFrom.Configuration(context.Configuration);
});
// Other configuration or startup code...
var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllers();

// Run the application locally (not for Lambda)
if (!app.Environment.IsProduction())
{
    app.Run();
}

