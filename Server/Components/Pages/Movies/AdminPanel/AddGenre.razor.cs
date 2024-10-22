using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MovieHub.Contracts.Requests;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages.Movies.AdminPanel
{
    public partial class AddGenre 
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

        protected string Error;
        protected bool ErrorVisible;
        protected NewGenre Genre;
        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected MovieService MovieService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (!Security.IsAdministrator())
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            Genre = new NewGenre();
        }

        protected async Task FormSubmit(NewGenre genre)
        {
            var request = new CreateGenreRequest
            {
                Name = genre.Name,
            };
            var response = await MovieService.CreateGenre(request);
            if (response.IsSuccess)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Success", "Genre added successfully");
                DialogService.Close(null);
            }
            else
            {
                foreach (var errorMessage in response.ErrorMessages)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error, Summary = "Error", Detail = errorMessage,
                        Duration = 8000
                    });
                }
            }
        }

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
        protected class NewGenre
        {
            public string Name { get; set; }
        }

    }
    
}