using Microsoft.EntityFrameworkCore;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.AsyncDataServices;
using CommandService.SyncDataServices.Grpc;
// using CommandService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); // 👈 add this line

// Add services to the container.
builder.Services.AddControllers(); //  moved here
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

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

  Console.WriteLine("--> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));


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

PrepDb.PrepPopulation(app);

app.Run();
