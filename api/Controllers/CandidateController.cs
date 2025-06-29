using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly IMongoCollection<Candidate> _collection;

    // Dependency Injection
    public CandidateController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Candidate>("candidates");
    }

    [HttpPost("register")]
    public ActionResult<Candidate> Register(Candidate candidateInput)
    {
        Candidate? candidate = _collection.Find(
            doc => doc.NationalCode == candidateInput.NationalCode).FirstOrDefault();

        if (candidate is not null)
        {
            return BadRequest("NationalCode is already exist");
        }

        _collection.InsertOne(candidateInput);

        return candidateInput;
    }

    [HttpGet("get-all")]
    public ActionResult<List<Candidate>> GetAll()
    {
        List<Candidate> candidates = _collection.Find(new BsonDocument()).ToList();

        if (candidates.Count == 0)
        {
            return NoContent();
        }

        return candidates;
    }

    [HttpGet("get-by-nationalcode/{nationalCode}")]
    public ActionResult<Candidate> GetCandidateByNationalCode(string nationalCode)
    {
        Candidate? candidate = _collection.Find(
            doc => doc.NationalCode == nationalCode).FirstOrDefault();

        if (candidate is null)
        {
            return NotFound("Candidate not found");
        }

        return candidate;
    }

    [HttpPut("update/{candidateId}")]
    public ActionResult<UpdateResult> UpdateCandidateById(string candidateId, Candidate candidateInput)
    {
        Candidate? candidate = _collection.Find(
            doc => doc.Id == candidateId).FirstOrDefault();

        if (candidate is null)
        {
            return NotFound("Candidate not found");
        }

        UpdateDefinition<Candidate> updatedCandidate = Builders<Candidate>.Update
        .Set(doc => doc.NationalCode, candidateInput.NationalCode)
        .Set(doc => doc.DegreeOfEducation, candidateInput.DegreeOfEducation);

        return _collection.UpdateOne(doc => doc.Id == candidateId, updatedCandidate);
    }

    [HttpDelete("delete/{candidateId}")]
    public ActionResult<DeleteResult> DeleteCandidateById(string candidateId)
    {
        Candidate? candidate = _collection.Find(
            doc => doc.Id == candidateId).FirstOrDefault();

        if (candidate is null)
        {
            return NotFound("Candidate not found");
        }

        return _collection.DeleteOne(doc => doc.Id == candidateId);
    }
}
