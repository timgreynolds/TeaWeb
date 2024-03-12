using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaApi.Models;
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

        public TeasController(IDataService<TeaModel> dataService, ILogger<TeasController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dataService = dataService;
            try
            {
                _dataService.Initialize(configuration.GetConnectionString("SqliteDB") ?? String.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Could not initialize the tea database. {Type} - {Message}", ex.GetType().Name, ex.Message);
            }
        }

        [HttpGet, ActionName("GetTeas")]
        public async Task<IActionResult> GetTeas()
        {
            _logger.LogInformation("Entering action: {Action}", nameof(GetTeas));
            try
            {
                TeaResponse response;
                List<TeaModel> teas = await _dataService.GetAsync();
                if (teas.Count < 1)
                {
                    response = new TeaResponse { Success = false, Message = "No teas found in the database." };
                    return new NotFoundObjectResult(response);
                }
                response = new TeaResponse { Success = true, Teas = teas };
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        [HttpGet("{id}"), ActionName("GetTea")]
        public async Task<IActionResult> GetTea(int id)
        {
            _logger.LogInformation("Entering action: {Action}", nameof(GetTea));
            try
            {
                TeaResponse response;
                TeaModel tea = await _dataService.FindByIdAsync(id);
                if (tea is null)
                {
                    _logger.LogWarning("No tea found with ID {TeaId}", id);
                    response = new TeaResponse { Success = false, Message = $"No tea found with ID {id}" };
                    return new NotFoundObjectResult(response);
                }
                response = new TeaResponse { Success = true, Teas = new List<TeaModel> { tea } };
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        [HttpPost, ActionName("AddTea")]
        public async Task<IActionResult> AddTea([FromBody] JsonElement body)
        {
            _logger.LogInformation("Entering action: {Action}", nameof(AddTea));
            try
            {
                TeaModel? value = JsonSerializer.Deserialize<TeaModel>(body);
                TeaModel tea = await _dataService.AddAsync(value);
                string? location = Url.Link("api", new { Controller = "Teas", id = tea.Id });
                TeaResponse response = new TeaResponse { Success = true, Teas = new List<TeaModel> { tea } };
                return new CreatedResult(location, response);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        [HttpPut, ActionName("UpdateTea")]
        public async Task<IActionResult> UpdateTea([FromBody] JsonElement body)
        {
            _logger.LogInformation("Entering action: {Action}", nameof(UpdateTea));
            try
            {
                TeaModel? value = JsonSerializer.Deserialize<TeaModel>(body);
                TeaModel tea = await _dataService.UpdateAsync(value);
                TeaResponse response = new TeaResponse { Success = true, Teas = new List<TeaModel>() { tea } };
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
        }

        [HttpDelete, ActionName("DeleteTea")]
        public async Task<IActionResult> DeleteTea([FromBody] JsonElement body)
        {
            _logger.LogInformation("Entering action: {Action}", nameof(DeleteTea));
            TeaModel? value;
            TeaResponse response;
            bool deleted;
            try
            {
                value = JsonSerializer.Deserialize<TeaModel>(body);
                deleted = await _dataService.DeleteAsync(value);
            }
            catch (Exception ex)
            {
                return ExceptionResponse(ex);
            }
            if (deleted == false)
            {
                _logger.LogWarning("Tea {Name} could not be deleted.", value.Name);
                response = new TeaResponse { Success = false, Message = $"Tea {value.Name} could not be deleted." };
                return new BadRequestObjectResult(response);
            }
            response = new TeaResponse { Success = true, Message = $"Tea {value.Name} deleted." };
            return new OkObjectResult(response);
        }

        private ObjectResult ExceptionResponse(Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            TeaResponse response = new TeaResponse { Success = false, Message = ex.Message };
            return StatusCode((int)HttpStatusCode.InternalServerError, response);
        }
    }
}
