using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace MovieHub.Components.Pages.ApplicationUser
{
    public partial class EditApplicationUser
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
        protected MovieHub.Models.ApplicationUser User;
        protected IEnumerable<string> UserRoles;
        protected string Error;
        protected bool ErrorVisible;

        [Parameter]
        public string Id { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!Security.IsAdministrator())
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            User = await Security.GetUserById($"{Id}");

            UserRoles = User.Roles.Select(role => role.Id);

            Roles = await Security.GetRoles();
        }

        protected async Task FormSubmit(MovieHub.Models.ApplicationUser user)
        {
            try
            {
                user.Roles = Roles.Where(role => UserRoles.Contains(role.Id)).ToList();
                await Security.UpdateUser($"{Id}", user);
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