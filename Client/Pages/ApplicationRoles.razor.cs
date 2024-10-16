using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MovieHub.Client.Services;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Client.Pages
{
    public partial class ApplicationRoles
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected IEnumerable<MovieHub.Server.Models.ApplicationRole> Roles;
        protected RadzenDataGrid<MovieHub.Server.Models.ApplicationRole> Grid0;
        protected string Error;
        protected bool ErrorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Roles = await Security.GetRoles();
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationRole>("Add Application Role");

            Roles = await Security.GetRoles();
        }

        protected async Task DeleteClick(MovieHub.Server.Models.ApplicationRole role)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this role?") == true)
                {
                    await Security.DeleteRole($"{role.Id}");

                    Roles = await Security.GetRoles();
                }
            }
            catch (Exception ex)
            {
                ErrorVisible = true;
                Error = ex.Message;
            }
        }
    }
}