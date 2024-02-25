using com.mahonkin.tim.TeaDataService.Services;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Services.TeaSqLiteService;
using com.mahonkin.tim.logging.UnifiedLogging.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.mahonkin.tim.TeaWeb.TeaApi;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IDataService<TeaModel>, TeaSqlService>();

        builder.Services.AddControllers();
        builder.Logging.ClearProviders()
            .AddConsole()
            .AddUnifiedLogger();
        
        WebApplication app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
