using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.mahonkin.tim.TeaDataService.DataModel;
using Microsoft.AspNetCore.Components;

namespace com.mahonkin.tim.TeaUI.Pages
{
    public partial class Index
    {
        private static RenderFragment? renderValidationErrors;
        private static List<TeaModel> teas = new List<TeaModel>();
        private static string teaName = string.Empty;
        private static string teaSteepTime = "02:00";
        private static int teaBrewTemp = 212;

        protected override async Task OnInitializedAsync()
        {
            teas = await SqlService.GetAsync();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            teas = await SqlService.GetAsync();
            renderValidationErrors = null;
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task AddTeaAsync()
        {
            try
            {
                TeaModel newTea = await SqlService.AddAsync(new TeaModel(teaName, teaSteepTime, teaBrewTemp));
            }
            catch (Exception exception)
            {
                RenderException(exception);
            }
            finally
            {
                teas = await SqlService.GetAsync();
                teaName = string.Empty;
                teaBrewTemp = 212;
                teaSteepTime = "02:00";
            }
        }

        private async Task EditTeaAsync(TeaModel tea)
        {
            await Task.Run(() => Console.WriteLine(tea.Name));
        }

        private async Task DeleteTeaAsync(TeaModel tea)
        {
            try
            {
                if (await SqlService.DeleteAsync(tea) == false)
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
                teas = await SqlService.GetAsync();
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

