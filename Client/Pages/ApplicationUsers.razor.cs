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
    public partial class ApplicationUsers
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

        protected IEnumerable<MovieHub.Server.Models.ApplicationUser> Users;
        protected RadzenDataGrid<MovieHub.Server.Models.ApplicationUser> Grid0;
        protected string Error;
        protected bool ErrorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Users = await Security.GetUsers();
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationUser>("Add Application User");

            Users = await Security.GetUsers();
        }

        protected async Task RowSelect(MovieHub.Server.Models.ApplicationUser user)
        {
            await DialogService.OpenAsync<EditApplicationUser>("Edit Application User", new Dictionary<string, object>{ {"Id", user.Id} });

            Users = await Security.GetUsers();
        }

        protected async Task DeleteClick(MovieHub.Server.Models.ApplicationUser user)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
                {
                    await Security.DeleteUser($"{user.Id}");

                    Users = await Security.GetUsers();
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