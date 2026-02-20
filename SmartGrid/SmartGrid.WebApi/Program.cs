using Microsoft.OpenApi;
using SmartGrid.WebApi.Extensions;
using SmartGrid.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

var webApiConfig = builder.Configuration.GetSection("WebApi");
string corsPolicyName = webApiConfig.GetValue<string>("CorsPolicyName") ?? "_reactAppPolicy";
string reactOrigin = webApiConfig.GetValue<string>("ReactOrigin") ?? "http://localhost:5173";
string deviceHubRoute = webApiConfig.GetValue<string>("DeviceHubRoute") ?? "/device-status-hub";

builder.Services
    .AddWebApiServices(builder.Configuration)
    .AddWebApiCors(reactOrigin)
    .AddWebApiSignalR()
    .AddWebApiHostedServices();

var app = builder.Build();

app.UseCors(corsPolicyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<DeviceHub>(deviceHubRoute);
app.MapControllers();

app.Run();
