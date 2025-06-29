using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoterController : ControllerBase
{
    private readonly IMongoCollection<Voter> _collection;

    // Dependency Injection
    public VoterController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Voter>("voters");
    }

    [HttpPost("submit-vote")]
    public ActionResult<Voter> SubmitVote(Voter voterInput)
    {
        _collection.InsertOne(voterInput);

        return voterInput;
    }

    [HttpGet("get-vote-by-nationalcode/{nationalCode}")]
    public ActionResult<List<Voter>> GetVoterVotes(string nationalCode)
    {
        List<Voter> votes = _collection.Find(
            doc => doc.NationalCode == nationalCode).ToList();

        if (votes.Count == 0)
        {
            return NoContent();
        }

        return votes;
    }

    [HttpGet("get-by-vots/{inputId}")]
    public ActionResult<List<Voter>> GetVotesCandidate(string inputId)
    {
        List<Voter> votes = _collection.Find(doc => doc.CandidateId == inputId).ToList();

        if (votes.Count == 0)
            return NoContent();

        return votes;
    }
}