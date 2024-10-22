using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Components.Pages.ApplicationRoles
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

        protected IEnumerable<MovieHub.Models.ApplicationRole> Roles;
        protected RadzenDataGrid<MovieHub.Models.ApplicationRole> Grid0;
        protected string Error;
        protected bool ErrorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!Security.IsAdministrator())
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            Roles = await Security.GetRoles();
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationRole>("Add Application Role");

            Roles = await Security.GetRoles();
        }

        protected async Task DeleteClick(MovieHub.Models.ApplicationRole role)
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