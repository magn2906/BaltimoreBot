using BaltimoreBot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace BaltimoreBot.Handlers.SlashCommands;

public class ListBannedWords : ApplicationCommandModule
{
    private readonly IBannedWordService _bannedWordService;
    
    public ListBannedWords(IBannedWordService bannedWordService)
    {
        _bannedWordService = bannedWordService;
    }

    [SlashCommand("list-banned-words", "Lists all banned words")]
    public async Task ListBannedWordsCommand(InteractionContext ctx)
    {
        var bannedWords = _bannedWordService.GetAllBannedWords();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
        {
            Content = "Banned words: " + string.Join("\n", bannedWords)
        });
    }
}