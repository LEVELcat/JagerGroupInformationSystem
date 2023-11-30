using JagerGroupIS.DatabaseContext;
using JagerGroupIS.DiscordBot;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.Configure<ConnectionString>(builder.Configuration.GetSection("ConnectionStrings"));
//builder.Services.Configure<DiscordToken>(builder.Configuration.GetSection("DiscordToken"));

DiscordBotDbContext.ConnectionString = builder.Configuration.GetValue<string>("ConnectStrings");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DiscordBotDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

DiscordBot.AsyncMain(builder.Configuration);

var client = DiscordBot.Client;

app.Run();

DiscordBot.Close();