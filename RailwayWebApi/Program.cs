using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RailwayWebApi.Data;
using RailwayWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb"))
);

// 2️⃣ Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

// 3. JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddSingleton<TokenService>();

// 3️⃣ Add controllers
builder.Services.AddControllers();

// 4️⃣ Add Swagger (replace AddOpenApi)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Railway API", Version = "v1" });
});


var app = builder.Build();

// 5️⃣ Configure Swagger middleware (replace MapOpenApi)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Railway API V1");
        c.RoutePrefix = string.Empty; // optional, makes Swagger UI available at root
    });
}

// 6️⃣ Other middlewares
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 7️⃣ Map controllers
app.MapControllers();

app.Run();
