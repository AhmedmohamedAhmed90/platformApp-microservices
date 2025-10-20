using Microsoft.EntityFrameworkCore;
using CommandService.Data;
// using CommandService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers(); //  moved here
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommandRepo, CommandRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// Configure DbContext based on environment
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}
else
{
    // Console.WriteLine("--> Using SQL Server DB");
    // builder.Services.AddDbContext<AppDbContext>(opt =>
    //     opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();                   // expose controller routes

app.Run();
