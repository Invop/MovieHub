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
    public partial class AddApplicationUser
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
        protected MovieHub.Server.Models.ApplicationUser User;
        protected IEnumerable<string> UserRoles = Enumerable.Empty<string>();
        protected string Error;
        protected bool ErrorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = new MovieHub.Server.Models.ApplicationUser();

            Roles = await Security.GetRoles();
        }

        protected async Task FormSubmit(MovieHub.Server.Models.ApplicationUser user)
        {
            try
            {
                user.Roles = Roles.Where(role => UserRoles.Contains(role.Id)).ToList();
                await Security.CreateUser(user);
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