using JagerGroupIS.DatabaseContext;
using JagerGroupIS.DiscordBot;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

//DiscordBot.AsyncMain(args);

using (DiscordBotDbContext discordBot = new DiscordBotDbContext("test"))
{
    //discordBot.Database.EnsureDeleted();

    discordBot.Database.EnsureCreated();

    Console.WriteLine("Hello World");
}

app.Run();
