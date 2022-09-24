using CRS.Shared;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using NRCReg;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddDbContext<DatabaseContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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