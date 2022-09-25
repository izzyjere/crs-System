using CRS.Shared;
using CRS.Web;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using System.Text;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<DatabaseContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), o2 =>
    {
        o2.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

builder.Services.AddMudServices();

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
app.MapPost("/api/suspects", (DatabaseContext db, [FromBody] SuspectRequest request) =>
{
    var suspect = new Suspect
    {
        Name = request.Name,
        NRC = request.NRC,
        Complexion = request.Complexion,
        EyeColor = request.EyeColor,
        Occupation = request.Occupation,
        PhysicalAddress = request.PhysicalAddress,
        Biometrics = new()
    };
    for (int i = 0; i < 10; i++)
    {
        suspect.Biometrics.Add(new Biometric() { Data = request.Bytes[i] });
    }
    if (db.Suspects.Any(s => s.NRC == request.NRC))
    {
        return Result.Fail("Suspect already exists.");
    }
    db.Suspects.Add(suspect);
    return db.SaveChanges() !=0 ? Result.Success("Saved successfully") : Result.Fail( "An error occured try again.");
});
app.MapPost("/api/suspects/{nrc}", async (DatabaseContext db, [FromBody] string nrc) =>
{
    var suspect = await db.Suspects.Include(s => s.Cases).FirstOrDefaultAsync(s => s.NRC==nrc);
    if(suspect == null)
    {
        return Result<SuspectResponse2>.Fail("Not found.");
    }
    return Result<SuspectResponse2>.Success(new SuspectResponse2 { Cases = suspect.Cases.Count, Name = suspect.Name, NRC = suspect.NRC });
});
app.Run();
record SuspectRequest
{      
    public List<byte[]> Bytes { get; set; }
    public string EyeColor { get; set; }
    public string NRC { get; set; }
    public string Name { get; set; }
    public string Complexion { get; set; }
    public string Occupation { get; set; }
    public string PhysicalAddress { get; set; }
}
public record SuspectResponse
{
    public Suspect Suspect { get; set; }
    public PersonalDetails PersonalDetails { get; set; }
}
public record SuspectResponse2
{
    public string Name { get; set; }
    public string NRC { get; set; } 
    public int Cases { get; set; }
    public string HasCrimes => Cases >=1?"Yes":"No";

}