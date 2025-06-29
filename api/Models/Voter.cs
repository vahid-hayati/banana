using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

public record Voter(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    string? Id,
    string FirstName,
    string LastName,
    string NationalCode,
    string CandidateId,
    Address CompleteAddress
);
