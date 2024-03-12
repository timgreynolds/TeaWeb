using System.Collections.Generic;
using com.mahonkin.tim.TeaDataService.DataModel;

namespace com.mahonkin.tim.TeaApi.Models;

public class TeaResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public List<TeaModel> Teas { get; set; } = new List<TeaModel>();
}
