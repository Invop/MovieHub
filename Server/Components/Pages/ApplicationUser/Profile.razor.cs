using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace MovieHub.Components.Pages.ApplicationUser
{
    public partial class Profile
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

        protected string OldPassword = "";
        protected string NewPassword = "";
        protected string ConfirmPassword = "";
        protected MovieHub.Models.ApplicationUser User;
        protected string Error;
        protected bool ErrorVisible;
        protected bool SuccessVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await Security.GetUserById($"{Security.User.Id}");
        }

        protected async Task FormSubmit()
        {
            try
            {
                await Security.ChangePassword(OldPassword, NewPassword);
                SuccessVisible = true;
            }
            catch (Exception ex)
            {
                ErrorVisible = true;
                Error = ex.Message;
            }
        }
    }
}