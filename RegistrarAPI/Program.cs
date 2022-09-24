using CRS.Shared;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using RegistrarAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/api/generate",(DatabaseContext db) =>
{
    if(!db.NRCNumbers.Any())
    {
        int baseNumber = 100000;
        db.NRCNumbers.Add(new NRCNumber { Value = baseNumber });
        db.SaveChanges();
    }
    var last = db.NRCNumbers.First();
    last.Value++;
    db.NRCNumbers.Update(last);
    db.SaveChanges();
    return Result<string>.Success(data:$"{last.Value}/67/1");
});
app.MapPost("/api/create", ([FromBody] CitizenRequest request, DatabaseContext db) =>
{
    var citizen = new Citizen
    {
        PersonalDetails = new PersonalDetails()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfRegistration = request.DateOfRegistration,
            DateOfBirth = request.DateOfBirth,
            MiddleName = request.MiddleName,
            Gender = request.Gender,
            Village = request.Village,
            Chief = request.Chief,
            NRC = request.NRC,
            District = request.District,
            ThumbPrintData = new()
            {
                Data = request.ThumbPrintData
            }
        }

    };
    try
    {
        db.Citizens.Add(citizen);

        return db.SaveChanges()!=0 ? Result.Success() : Result.Fail();
    }
    catch (Exception)
    {

        return Result.Fail();
    }
});
app.UseHttpsRedirection();



app.Run();

class CitizenRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string NRC { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; }
    public string Chief { get; set; }
    public string Village { get; set; }
    public string District { get; set; }
    public DateTime DateOfRegistration { get; set; }
    public int Id { get; set; }
    public string FullName => FirstName + " "+MiddleName +" "+ LastName;
    public byte[] ThumbPrintData { get; set; }
}