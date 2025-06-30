using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

// Model | Entity => save the DataBase  
public record AppUser(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)] string? Id, // hamishe sabet
    string Email,
    string UserName, //property
    [Range(18, 70)] int Age,
    [Length(4, 8)] string Password,
    string ConfirmPassword,
    string Gender,
    string City,
    string Country
);