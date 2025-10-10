var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();      // enable controller discovery
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();       // or builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();                   // expose controller routes

app.Run();
