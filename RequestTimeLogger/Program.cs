using NLog;
using NLog.Web;
using RequestTimeLogger.Infraestructure.Middleware;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");


try
{
	var builder = WebApplication.CreateBuilder(args);


	// Add services to the container.

	builder.Services.AddControllers();
	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	//Configure Logging
	builder.Logging.ClearProviders();
	builder.Host.UseNLog();

    var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseStaticFiles();

    app.UseMiddleware<RequestLoggerMiddleware>();

    app.UseRouting();

	app.UseAuthorization();

	app.MapControllers();

	app.Run();

}
catch(Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}