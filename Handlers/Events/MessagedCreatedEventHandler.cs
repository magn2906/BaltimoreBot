using System.Text.RegularExpressions;
using BaltimoreBot.Services;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace BaltimoreBot.Handlers.Events;

public class MessagedCreatedEventHandler
{
    private readonly IBannedWordService _bannedWordService;

    public MessagedCreatedEventHandler(IBannedWordService bannedWordService)
    {
        _bannedWordService = bannedWordService;
    }

    public async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
    {
        if (e.Author.IsBot)
        {
            return;
        }

        var bannedWords = _bannedWordService.GetAllBannedWords();
        var messageContent = e.Message.Content;

        var isBannedWordContained = CheckForBannedWords(messageContent, bannedWords, out var bannedWord);

        if (isBannedWordContained)
        {
            var pattern = @"\b" + string.Join(@"\s*", bannedWord.ToCharArray()) + @"\b";
            var encapsulatedMessage =
                Regex.Replace(messageContent, pattern, $"||{bannedWord}||", RegexOptions.IgnoreCase);
            await e.Message.DeleteAsync();
            await e.Message.RespondAsync($"{e.Author.Username}: \"{encapsulatedMessage}\" contains a banned word");
        }
    }

    private static bool CheckForBannedWords(string messageContent, IEnumerable<string> bannedWords,
        out string bannedWord)
    {
        var sanitizedContentSpan = new Span<char>(new string(messageContent.Where(
            c => !char.IsWhiteSpace(c)).ToArray()).ToLower().ToCharArray());

        foreach (var word in bannedWords)
        {
            var sanitizedBannedWordSpan =
                new Span<char>(new string(word.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower().ToCharArray());
            if (!sanitizedContentSpan.ToString()
                    .Contains(sanitizedBannedWordSpan.ToString(), StringComparison.Ordinal))
            {
                continue;
            }

            bannedWord = word;
            return true;
        }

        bannedWord = string.Empty;
        return false;
    }
}