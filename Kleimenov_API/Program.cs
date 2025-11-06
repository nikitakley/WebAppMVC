using Kleimenov_API;
using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Kleimenov_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Kleimenov_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Kleimenov_APIContext") ?? throw new InvalidOperationException("Connection string 'Kleimenov_APIContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<DishService>();
builder.Services.AddScoped<CourierService>();
builder.Services.AddScoped<OrderStatusService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<PaymentService>();

// Для аутентификации
builder.Services.AddScoped<AuthService>();
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("AuthSettings"));

var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>()
    ?? throw new InvalidOperationException("AuthSettings не настроены");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authSettings.SecretKey))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Начальные записи в Users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Kleimenov_APIContext>();

    if (!db.Users.Any())
    {
        var adminAccount = new User
        {
            Username = "admin",
            Role = "Admin"
        };
        var adminPassword = new PasswordHasher<User>().HashPassword(adminAccount, "admin111");
        adminAccount.PasswordHash = adminPassword;
        db.Users.Add(adminAccount);

        var userAccount = new User
        {
            Username = "user",
            Role = "User"
        };
        var userPassword = new PasswordHasher<User>().HashPassword(userAccount, "user111");
        userAccount.PasswordHash = userPassword;
        db.Users.Add(userAccount);

        db.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
