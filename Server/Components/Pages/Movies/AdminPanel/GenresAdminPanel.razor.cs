using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MovieHub.Components.Pages.ApplicationRoles;
using MovieHub.Contracts.Responses;
using MovieHub.Services;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Components.Pages.Movies.AdminPanel;

public partial class GenresAdminPanel : ComponentBase
{
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }
    [Inject] protected ILogger<GenresAdminPanel> Logger { get; set; }

    protected RadzenDataGrid<GenreResponse> GenresGrid { get; set; }
    protected string Error;
    protected bool ErrorVisible;
    protected bool IsLoading = false;
    [Inject] protected SecurityService Security { get; set; }
    [Inject] protected MovieService MovieService { get; set; }
    
    
    protected GenresResponse Genres { get; set; }
    private IList<GenreResponse> _selectedGenres;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        var genresResponse = await MovieService.GetAllGenresAsync();
        Genres = genresResponse.Data;
        IsLoading = false;
    }

    private async Task LoadGenresAsync(LoadDataArgs args)
    {
        IsLoading = true;
        var genresResponse = await MovieService.GetAllGenresAsync();
        Genres = genresResponse.Data;
        IsLoading = false;
    }
    private async Task LoadGenresAsync()
    {
        var genresResponse = await MovieService.GetAllGenresAsync();
        Genres = genresResponse.Data;
    }
    protected async Task AddClick()
    {
        await DialogService.OpenAsync<AddGenre>("Add Genre");

    }
    protected async Task EditClick()
    {
        try
        {
            var genreToEdit = GetSelectedGenre();
            if (genreToEdit == null) return;

            
            await DialogService.OpenAsync<EditGenre>("Edit Genre", CreateDialogParameters(genreToEdit.Id));
            await LoadGenresAsync();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Edit Genre Failed");
        }
    }
    protected async Task DeleteClick()
    {
        var genreToDelete = GetSelectedGenre();
        if (genreToDelete == null) 
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Warning", "No genre selected");
            return;
        }
    
        bool? confirmResult = await DialogService.Confirm($"Are you sure you want to delete this genre({genreToDelete?.Name})?");
        if (confirmResult == false) return;

        var response = await MovieService.DeleteGenre(genreToDelete.Id);
        if (response.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Genre deleted successfully");
            await LoadGenresAsync();
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
    private void HandleError(Exception ex, string errorMessage)
    {
        Logger.LogError(ex, errorMessage);
        ErrorVisible = true;
        Error = errorMessage;
    }
    private Dictionary<string, object> CreateDialogParameters(int genreId)
    {
        return new Dictionary<string, object> { { "Id", genreId } };
    }
    private GenreResponse GetSelectedGenre()
    {
        return _selectedGenres?.FirstOrDefault();
    }
    private void ClearSelection()
    {
        _selectedGenres = null;
    }

}