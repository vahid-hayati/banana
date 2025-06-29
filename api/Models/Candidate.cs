using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

public record Candidate(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)]
    string? Id,
    string FirstName,
    string LastName,
    [Length(10, 10)]string NationalCode,
    string DegreeOfEducation,
    int Age,
    Address CompleteAddress // nested object
);
