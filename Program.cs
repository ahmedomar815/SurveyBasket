using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket;
using SurveyBasket.Entities;
using SurveyBasket.Persistence;
using System.Reflection;





var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddHybridCache();
// Configure Serilog for ASP.NET Core
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration);
        
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
