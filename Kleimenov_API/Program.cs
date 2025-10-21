using Microsoft.EntityFrameworkCore;
using Kleimenov_API.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Kleimenov_APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Kleimenov_APIContext") ?? throw new InvalidOperationException("Connection string 'Kleimenov_APIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
