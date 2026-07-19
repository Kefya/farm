using FarmGame.Api.Data;
using FarmGame.Api.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder =
WebApplication.CreateBuilder(args);

// Configuration
var configuration =
builder.Configuration;

// DbContext
builder.Service.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
// добавить домены

builder.Services.AddControllers();
builder.Services.AddEndpointApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var jwtSection = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new Exception("Jwt:Key is missing"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();