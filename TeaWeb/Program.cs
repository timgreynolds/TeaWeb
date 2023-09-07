using com.mahonkin.tim.TeaDataService.Services;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Services.TeaSqLiteService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging(conf =>
{
    conf.ClearProviders();  
    conf.AddSimpleConsole();
    //conf.AddJsonConsole();
});
builder.Services.AddSingleton<IDataService<TeaModel>, TeaSqlService>();

WebApplication app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();

