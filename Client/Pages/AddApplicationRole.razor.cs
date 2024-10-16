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
    public partial class AddApplicationRole
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

        protected MovieHub.Server.Models.ApplicationRole Role;
        protected string Error;
        protected bool ErrorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Role = new MovieHub.Server.Models.ApplicationRole();
        }

        protected async Task FormSubmit(MovieHub.Server.Models.ApplicationRole role)
        {
            try
            {
                await Security.CreateRole(role);

                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                ErrorVisible = true;
                Error = ex.Message;
            }
        }

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}