using Adeverinte_Backend;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration; // get configuration from appsettings.json
var services = builder.Services;

builder.Services.AddControllers();
services.AddCors(options => options.AddPolicy("EnableAll", policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
}));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositories(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("EnableAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
