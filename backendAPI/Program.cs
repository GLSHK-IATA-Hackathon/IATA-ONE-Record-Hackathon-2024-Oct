using Microsoft.OpenApi.Models;
using System.Reflection;
using WebAPITemplate.Extension;
using WebAPITemplate.Interface;
using WebAPITemplate.Service;
using WebAPITemplate.Swagger;
using static WebAPITemplate.Models.JMCommon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configNEOneAPIService = builder.Configuration.GetValue<string>("APIConfig:NEOneAPI:BaseUrl");


//builder.Services.Configure<NEOneAPIConfig>(Configuration.GetSection("AppSettings").GetSection("NEOneAPIConfig"));

builder.Services.AddHttpClient<INEOneAPIService, NEOneAPIService>(x =>
{
    x.BaseAddress = new Uri(configNEOneAPIService);

    //x.DefaultRequestHeaders.Add("authorization", "Bearer {CannelAccessToken}");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((c =>
{
    c.OperationFilter<SwaggerExampleJsonRequestOperationFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
}));

//include all configure.
//e.g.: log4net.config, DataSource.config
builder.Host.AddHostConfigure();

//include all DI
builder.Services.AddDIServices();

//add validation
builder.Services.AddValidationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("AllowAllOrigin");
app.UseAuthorization();
app.MapControllers();
app.Run();

