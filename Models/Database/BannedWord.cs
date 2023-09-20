using MongoDB.Bson.Serialization.Attributes;

namespace BaltimoreBot.Models.Database;

public class BannedWord
{
    [BsonId] public Guid Id { get; set; } = Guid.NewGuid();
    public required string Word { get; set; }
}