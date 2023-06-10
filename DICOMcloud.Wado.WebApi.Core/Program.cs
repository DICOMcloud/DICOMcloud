using DICOMcloud.Wado;
using System.Web.Http;
using System.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Init();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.Environment.EnsureCodecsLoaded();

app.UseHttpsRedirection();

app.UseAuthorization();

app.RegisterRoutes();

app.UseCors("AllowCores");

app.Run();
