using NLog;
using LogLevel = NLog.LogLevel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "api.xml");
    c.IncludeXmlComments(filePath);
});

LogManager.Setup().LoadConfiguration(builder => {
   builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
   builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "logfile.nlog");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    LogManager.GetCurrentClassLogger().Error(ex);
}