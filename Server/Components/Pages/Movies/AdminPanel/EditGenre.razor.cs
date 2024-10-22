using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MovieHub.Contracts.Requests;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages.Movies.AdminPanel;

public partial class EditGenre : ComponentBase

{
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }

    protected string Error;
    protected bool ErrorVisible;
    protected GenreToUpdate Genre;
    [Inject] protected SecurityService Security { get; set; }
    [Inject] protected MovieService MovieService { get; set; }
    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!Security.IsAdministrator())
        {
            NavigationManager.NavigateTo("/unauthorized");
        }
        var genreResponse = await MovieService.GetGenre(Id.ToString());
        Genre = new GenreToUpdate
        {
            Id = genreResponse.Data.Id,
            Name = genreResponse.Data?.Name ?? string.Empty
        };
    }

    protected async Task FormSubmit(GenreToUpdate genre)
    {
        var request = new UpdateGenreRequest()
        {
            NewName = genre.Name,
        };
        var response = await MovieService.UpdateGenre(genre.Id, request);
        if (response.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Genre updated successfully");
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

    protected class GenreToUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}