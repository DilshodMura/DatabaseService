using DbService;
using DbService.Interfaces;
using ExternalDb;
using InternalDb;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISynchronizationService, SynchronizationService>();

//builder.Services.AddDbContext<InternalDbContext>(option =>
//    option.UseSqlServer(builder.Configuration.GetConnectionString("Data source = nba-091-01-UZ\\SQLEXPRESS; Database= internalDb; Integrated Security=True;TrustServerCertificate=True")));

//builder.Services.AddDbContext<ExternalDbContext>(option =>
//    option.UseSqlServer());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
