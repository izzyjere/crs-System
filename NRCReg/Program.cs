using CRS.Shared;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using NRCReg;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddDbContext<DatabaseContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), o2 =>
    {
        o2.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet("/api/nrc-number", (DatabaseContext db) =>
{
    if (!db.NRCNumbers.Any())
    {
        int baseNumber = 100000;
        db.NRCNumbers.Add(new NRCNumber { Value = baseNumber });
        db.SaveChanges();
    }
    var last = db.NRCNumbers.First();
    last.Value++;
    db.NRCNumbers.Update(last);
    db.SaveChanges();
    return Result<string>.Success(data: $"{last.Value}/67/1");
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
            PlaceOfBirth = request.PlaceOfBirth,
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
app.MapGet("/api/citizens", async (DatabaseContext db) =>
{
    return await db.Citizens.ToListAsync();
}); 
app.MapGet("/api/get-all", async (DatabaseContext db) =>
{
    var data = await db.Citizens.ToListAsync();
    var lis = new List<CitizenResponse>();
    foreach (var request in data.Select(r=>r.PersonalDetails))
    {
        var res = new CitizenResponse()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,            
            DateOfBirth = request.DateOfBirth,
            MiddleName = request.MiddleName,
            Gender = request.Gender,
            PlaceOfBirth = request.PlaceOfBirth,
            Village = request.Village,
            Chief = request.Chief,
            NRC = request.NRC,
            District = request.District,
            Id = request.CitizenId,
            FingerPrintData = request.ThumbPrintData.Data
        };
        lis.Add(res);
    }
    return Result<List<CitizenResponse>>.Success(lis);
});
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
class CitizenResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string NRC { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; }
    public string Chief { get; set; }
    public string Village { get; set; }
    public string District { get; set; }
    public byte[] FingerPrintData { get; set; }

}