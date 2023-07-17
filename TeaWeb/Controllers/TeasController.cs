﻿using System.Collections.Generic;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.Services;
using com.mahonkin.tim.TeaDataService.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace com.mahonkin.tim.TeaApi.Controllers
{
    [Route("")]
    [Route("api")]
    [Route("api/[controller]")]
    public class TeasController : ControllerBase
    {
        private IDataService<TeaModel> _dataService;

        public TeasController(IDataService<TeaModel> dataService)
        {
            _dataService = dataService;
        }

        [HttpGet, ActionName("GetTeas")]
        public async Task<IActionResult> GetTeas()
        {
            List<TeaModel> teas = await _dataService.GetAsync();
            return new OkObjectResult(teas);
        }

        [HttpGet("{id}"), ActionName("GetTea")]
        public async Task<IActionResult> GetTea(int id)
        {
            TeaModel tea = await _dataService.FindByIdAsync(id);
            return new OkObjectResult(new[] { tea });
        }

        [HttpPost, ActionName("AddTea")]
        public async Task<IActionResult> AddTea([FromBody] TeaModel value)
        {
            TeaModel tea = await _dataService.AddAsync(value);
            return new OkObjectResult(new[] { tea });
        }

        [HttpPut(), ActionName("UpdateTea")]
        public async Task<IActionResult> UpdateTea([FromBody] TeaModel value)
        {
            TeaModel tea = await _dataService.UpdateAsync(value);
            return new OkObjectResult(tea);
        }

        [HttpDelete(), ActionName("DeleteTea")]
        public async Task<IActionResult> DeleteTea([FromBody] TeaModel value)
        {
            return new OkObjectResult(await _dataService.DeleteAsync(value));
        }
    }
}

