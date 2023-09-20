using System.Reflection;
using BaltimoreBot.Handlers;
using BaltimoreBot.Handlers.Events;
using BaltimoreBot.Services;
using BaltimoreBot.Utilities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = EnvironmentHandler.GetEnvironmentVariable("BaltimoreBotToken"),
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.All
});

var services = new ServiceCollection();
services.AddSingleton<IBannedWordService, BannedWordService>();
services.AddSingleton<MessagedCreatedEventHandler>();

var builtServices = services.BuildServiceProvider();

// Event handlers
var messagedCreatedEventHandler = builtServices.GetRequiredService<MessagedCreatedEventHandler>();
discord.MessageCreated += messagedCreatedEventHandler.OnMessageCreated;

// Slash commands
var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
{
    Services = builtServices
});

slash.RegisterCommands(Assembly.GetExecutingAssembly());


await discord.ConnectAsync();
await Task.Delay(-1);