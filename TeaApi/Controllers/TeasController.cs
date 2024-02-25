using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using com.mahonkin.tim.TeaDataService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace com.mahonkin.tim.TeaApi.Controllers
{
    [Route("")]
    [Route("api")]
    [Route("api/[controller]")]
    public class TeasController : ControllerBase
    {
        private readonly IDataService<TeaModel> _dataService;
        private readonly ILogger _logger;
        
        public TeasController(IDataService<TeaModel> dataService, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _dataService = dataService;
            _dataService.Initialize(configuration.GetConnectionString("SqliteDB") ?? String.Empty);
            _logger = loggerFactory.CreateLogger<TeasController>();
        }

        [HttpGet, ActionName("GetTeas")]
        public async Task<IActionResult> GetTeas()
        {
            _logger.LogInformation($"Entering action: { nameof(GetTeas) }");
            List<TeaModel> teas = await _dataService.GetAsync();
            return new OkObjectResult(teas);
        }

        [HttpGet("{id}"), ActionName("GetTea")]
        public async Task<IActionResult> GetTea(int id)
        {
            _logger.LogInformation($"Entering action: {nameof(GetTea)}");
            try
            {
                TeaModel tea = await _dataService.FindByIdAsync(id);
                if (tea is null)
                {
                    _logger.LogWarning($" No tea found with ID {id}");
                    return new NotFoundObjectResult($"No tea found with ID {id}");
                }
                return new OkObjectResult(new[] { tea });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return new NotFoundObjectResult(ex);
            }
        }

        [HttpPost, ActionName("AddTea")]
        public async Task<IActionResult> AddTea([FromBody] TeaModel value)
        {
            _logger.LogInformation($"Entering action: {nameof(AddTea)}");
            _logger.LogInformation($"Value from body: {value}");
            try
            {
                TeaModel tea = await _dataService.AddAsync(value);
                return new OkObjectResult(tea);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Console.WriteLine($"An error occurred. {ex.Message}");
                return new BadRequestObjectResult(ex);
            } 
        }

        [HttpPut(), ActionName("UpdateTea")]
        public async Task<IActionResult> UpdateTea([FromBody] TeaModel value)
        {
            _logger.LogInformation($"Entering action: {nameof(UpdateTea)}");
            TeaModel tea = await _dataService.UpdateAsync(value);
            return new OkObjectResult(tea);
        }

        [HttpDelete(), ActionName("DeleteTea")]
        public async Task<IActionResult> DeleteTea([FromBody] TeaModel value)
        {
            _logger.LogInformation($"Entering action: {nameof(DeleteTea)}");
            return new OkObjectResult(await _dataService.DeleteAsync(value));
        }
    }
}

