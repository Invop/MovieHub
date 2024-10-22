using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace MovieHub.Components.Pages.ApplicationRoles
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

        protected MovieHub.Models.ApplicationRole Role;
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
            Role = new MovieHub.Models.ApplicationRole();
        }

        protected async Task FormSubmit(MovieHub.Models.ApplicationRole role)
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