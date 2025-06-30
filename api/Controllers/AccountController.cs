using api.DTOs;
using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    #region dependency injections
    private readonly IMongoCollection<AppUser> _collection;
    // constructor - dependency injections
    public AccountController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<AppUser>("users");
    }
    #endregion

    //CRUD

    // [HttpPost("register")] //Crud
    // public ActionResult<LoggedInDto> Register(AppUser userInput) // userInput => name, age, password, confirmPassword
    // {
    //     if (userInput.Password != userInput.ConfirmPassword)
    //     {
    //         return BadRequest("Passwords do not match!");
    //     }

    //     AppUser user =
    //         _collection.Find(doc
    //          => doc.UserName == userInput.UserName).FirstOrDefault();

    //     if (user is not null)
    //     {
    //         return BadRequest("This username is already exist.");
    //     }

    //     _collection.InsertOne(userInput);

    //     LoggedInDto loggedInDto = new LoggedInDto(
    //         UserName: userInput.UserName
    //     );

    //     return loggedInDto;
    // }

    [HttpPost("register")] //Crud
    public ActionResult<LoggedInDto> Register(AppUser userInput) // userInput => name, age, password, confirmPassword
    {
        if (userInput.Password != userInput.ConfirmPassword)
        {
            return BadRequest("Passwords do not match!");
        }

        AppUser user =
            _collection.Find(doc
             => doc.UserName == userInput.UserName).FirstOrDefault();

        if (user is not null)
        {
            return BadRequest("This username is already exist.");
        }

        _collection.InsertOne(userInput);

        LoggedInDto loggedInDto = new LoggedInDto(
            UserName: userInput.UserName,
            Age: userInput.Age
        );

        return loggedInDto;
    }

    [HttpPost("login")]
    public ActionResult<LoggedInDto> Login(LoginDto userIn)
    {
        AppUser user = _collection.Find(doc =>
            doc.UserName == userIn.UserName && doc.Password == userIn.Password).FirstOrDefault();

        if (user is null)
        {
            return NotFound("User not found");
        }

        LoggedInDto loggedInDto = new LoggedInDto(
            UserName: user.UserName,
            Age: user.Age
        );

        return loggedInDto;

        // return user;
    }

    [HttpGet("get-all")]
    public ActionResult<List<MemberDto>> GetAll()
    {
        List<AppUser>? users = _collection.Find(new BsonDocument()).ToList();

        if (users.Count() == 0)
        {
            return NoContent();
        }

        List<MemberDto> memberDtos = [];

        foreach (AppUser user in users)
        {
            MemberDto memberDto = new MemberDto(
                Email: user.Email,
                UserName: user.UserName,
                Age: user.Age,
                Gender: user.Gender,
                City: user.City,
                Country: user.Country
            );

            memberDtos.Add(memberDto);
        }

        return memberDtos;
    }

    // http://localhost:5000/api/account/get-by-username/farzaneh
    [HttpGet("get-by-username/{userInput}")] //cRud
    public ActionResult<MemberDto> GetByUserName(string userInput)
    {
        AppUser? user = _collection.Find(
            doc => doc.UserName == userInput).FirstOrDefault();

        // default = null;

        if (user is null)
        {
            return NotFound("User not found.");
        }

        MemberDto memberDto = new MemberDto(
            Email: user.Email,
            UserName: user.UserName,
            Age: user.Age,
            Gender: user.Gender,
            City: user.City,
            Country: user.Country
        );

        return memberDto;
    }

    // Update / Put 
    [HttpPut("update-by-id/{userId}")]
    public ActionResult<UpdateResult> UpdateUserById(string userId, AppUser userInput)
    {
        AppUser? user = _collection.Find(doc => doc.Id == userId).FirstOrDefault();

        if (user is null)
        {
            return NotFound("User not found");
        }

        UpdateDefinition<AppUser> upatedUser = Builders<AppUser>.Update
        .Set(doc => doc.UserName, userInput.UserName)
        .Set(doc => doc.Age, userInput.Age);

        return _collection.UpdateOne(doc => doc.Id == userId, upatedUser);
    }

    [HttpDelete("delete-by-id/{userId}")]
    public ActionResult<DeleteResult> DeleteUserById(string userId)
    {
        AppUser? user = _collection.Find(doc => doc.Id == userId).FirstOrDefault();

        if (user is null)
        {
            return NotFound("User not found");
        }

        return _collection.DeleteOne(doc => doc.Id == userId);
    }
}