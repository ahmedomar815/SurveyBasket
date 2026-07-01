
using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SurveyBasket;
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
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
        [
             new HangfireCustomBasicAuthenticationFilter
             {
                 User=app.Configuration.GetValue<string>("HangfireSettings:Username"), 
                 Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
             }
        ],
        DashboardTitle = "Survey Basket Dashboard",
       // IsReadOnlyFunc=(DashboardContext context)=> true
});
var scopeFactory= app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope=scopeFactory.CreateScope();
var notificationService= scope.ServiceProvider.GetRequiredService<INotifiactionService>();
RecurringJob.AddOrUpdate("sendNewPollsNotification",()=>notificationService.SendNewPollsNoification(null), Cron.Daily);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
app.Run();
