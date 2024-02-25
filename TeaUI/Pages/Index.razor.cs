using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace com.mahonkin.tim.TeaUI.Pages
{
    public partial class Index : ComponentBase
    {
        private static RenderFragment? renderValidationErrors;
        private static List<TeaModel> teas = new List<TeaModel>();
        private static string teaName = string.Empty;
        private static string teaSteepTime = "02:00";
        private static int teaBrewTemp = 212;

        public bool ShowEditComponent = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string teaUrl = Config.GetConnectionString("TeaApiUrl") ?? "http://localhost/api";
                Logger.LogDebug($"Initializing URI {teaUrl}");
                DataService.Initialize(teaUrl.ToString());
                teas = await DataService.GetAsync();
            }
            catch (Exception exception)
            {
                RenderException(exception);
            }
            finally
            {
                await base.OnInitializedAsync();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            renderValidationErrors = null;
            try
            {
                teas = await DataService.GetAsync();
            }
            catch (Exception exception)
            {
                RenderException(exception);
            }
            finally
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }

        private async Task AddTeaAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teaName))
                {
                    throw new ArgumentNullException(nameof(teaName), "Tea must have a name.");
                }
                if (TimeSpan.TryParseExact(teaSteepTime, @"m\:ss", null, out TimeSpan steepTime) == false)
                {
                    throw new ArgumentException("Could not parse the specified steep time.", nameof(teaSteepTime));
                }
                TeaModel newTea = new TeaModel(teaName, steepTime, teaBrewTemp);
                newTea = await DataService.AddAsync(newTea);
            }
            catch (Exception exception)
            {
                RenderException(exception);
            }
            finally
            {
                teas = await DataService.GetAsync();
                teaName = string.Empty;
                teaBrewTemp = 212;
                teaSteepTime = "02:00";
            }
        }

        private void EditTeaAsync(TeaModel tea)
        {
            EditTea.SelectedTea = tea;
            ShowEditComponent = true;
        }

        private async Task DeleteTeaAsync(TeaModel tea)
        {
            try
            {
                if (await DataService.DeleteAsync(tea) == false)
                {
                    throw new ApplicationException("Tea could not be deleted.");
                }
            }
            catch (Exception exception)
            {
                RenderException(exception);
            }
            finally
            {
                teas = await DataService.GetAsync();
            }
        }

        private void RenderException(Exception exception)
        {
            renderValidationErrors = (builder) =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "id", "validation-error-ui");
                builder.AddAttribute(2, "class", "alert alert-danger");
                builder.AddAttribute(3, "role", "alert");
                builder.AddContent(4, $"{exception.Message}"); //<i class="bi bi-exclamation-triangle-fill"></i>
                builder.OpenElement(5, "button");
                builder.AddAttribute(6, "type", "button");
                builder.AddAttribute(7, "class", "btn-close float-end");
                builder.AddAttribute(8, "onclick", new EventCallbackFactory().Create(this, StateHasChanged));
                builder.CloseElement();
                builder.CloseElement();
            };
        }
    }
}

