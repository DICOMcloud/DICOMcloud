using BitMiracle.LibJpeg;
using DICOMcloud.Wado;
using DICOMcloud.Wado.WebApi.Core.Types;
using DICOMcloud.Wado.WebApi.Exceptions;
using FellowOakDicom;
using FellowOakDicom.Imaging.NativeCodec;
using Microsoft.VisualBasic;
using System;
using System.Web.Http;
using System.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

// Add MVC controllers 
builder.Services.AddControllers(options =>
{
    options.OutputFormatters.Add(new QidoResponseJSONOutputFormatter());
    options.OutputFormatters.Add(new QidoResponseXMLOutputFormatter());
    options.Filters.Add<DICOMcloudExceptionHandler>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.BuildDICOMcloud(builder);

new DicomSetupBuilder().RegisterServices(s => 
    s.AddFellowOakDicom()
    .AddLogging()
    .AddTranscoderManager<NativeTranscoderManager>())
    .Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.RegisterRoutes();

app.UseCors("AllowCors");

app.Run();
