using JagerGroupIS.DatabaseContext;
using JagerGroupIS.DiscordBot;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DiscordBotDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

string[] token = null;

using (DiscordBotDbContext discordBot = new DiscordBotDbContext())
{
    //discordBot.Database.EnsureDeleted();

    //discordBot.Database.EnsureCreated();

    token = discordBot.DiscordTokens.Select(x => x.Token).ToArray();
}

DiscordBot.AsyncMain(token);

var client = DiscordBot.Client;

app.Run();
