using CRS.Shared;
using CRS.Web;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<DatabaseContext>(o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
    foreach (var data in request.Bytes)
    {
        suspect.Biometrics.Add(new Biometric() { Data = data});
       
    }
    db.Suspects.Add(suspect);
    return db.SaveChanges() !=0 ? Result.Success() : Result.Fail();
});
app.MapGet("/api/suspects/{nrc}", async (DatabaseContext db, [FromRoute] string nrc) =>
{
    var sus = await db.Suspects.Include(s => s.Cases).ThenInclude(c => c.Judgements).FirstOrDefaultAsync(s => s.NRC==nrc);
    return sus!=null? Result<Suspect>.Success(sus):Result<Suspect>.Fail("Not found.");
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