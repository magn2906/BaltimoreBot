using MongoDB.Driver;
using BaltimoreBot.Models.Database;
using BaltimoreBot.Utilities;
using MongoDB.Bson;

namespace BaltimoreBot.Services;

public interface IBannedWordService
{
    Task AddBannedWordAsync(BannedWord word);
    Task RemoveBannedWordAsync(string word);
    IReadOnlyCollection<string> GetAllBannedWords();
}

public class BannedWordService : IBannedWordService
{
    private readonly IMongoCollection<BannedWord> _bannedWords;
    private readonly List<string> _bannedWordCache = new List<string>();


    public BannedWordService()
    {
        var connectionString = EnvironmentHandler.GetEnvironmentVariable("BaltimoreBotMongoConnectionString");
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("BaltimoreThursday");
        _bannedWords = database.GetCollection<BannedWord>("BannedWords");

        InitializeBannedWordCache().Wait();
    }

    private async Task InitializeBannedWordCache()
    {
        var words = await _bannedWords.Find(new BsonDocument()).ToListAsync();
        _bannedWordCache.AddRange(words.Select(w => w.Word));
    }

    public async Task AddBannedWordAsync(BannedWord word)
    {
        await _bannedWords.InsertOneAsync(word);
        _bannedWordCache.Add(word.Word);
    }

    public async Task RemoveBannedWordAsync(string word)
    {
        var filter = Builders<BannedWord>.Filter.Eq(w => w.Word, word);
        await _bannedWords.DeleteOneAsync(filter);
        _bannedWordCache.Remove(word);
    }

    public IReadOnlyCollection<string> GetAllBannedWords()
    {
        return _bannedWordCache.AsReadOnly();
    }
}