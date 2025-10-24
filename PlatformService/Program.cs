using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using PlatfromService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); //  moved here
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// Configure DbContext based on environment
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}
else
{
    Console.WriteLine("--> Using SQL Server DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}

var app = builder.Build();

// 🔹 Configure the middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

//just for testing 

// app.UseCors(allowAnyOrigin =>
// {
//     allowAnyOrigin
//         .AllowAnyOrigin()
//         .AllowAnyMethod()
//         .AllowAnyHeader();
// });

app.MapControllers(); //  map routes to controllers
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/Protos/Platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/Platforms.proto"));
});

Console.WriteLine($"-->CommandService Endpoint {builder.Configuration["CommandService"]}");

// Seed the database
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
