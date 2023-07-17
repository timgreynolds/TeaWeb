using com.mahonkin.tim.TeaDataService.Services;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.SqLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IDataService<TeaModel>, TeaSqlService<TeaModel>>();

WebApplication app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();

