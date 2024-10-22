using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MovieHub.Components.Pages.ApplicationRoles;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using MovieHub.Services;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Components.Pages.Movies.AdminPanel;

public partial class MoviesAdminPanel
{
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }

    [Inject] protected ILogger<MoviesAdminPanel> Logger { get; set; }

    protected string Error;
    protected bool ErrorVisible;
    protected bool IsLoading = false;
    protected RadzenDataGrid<MovieResponse> AdminMoviesGrid;
    private IList<MovieResponse> _selectedMovie;
    [Inject] protected SecurityService Security { get; set; }

    [Inject] protected MovieService MovieService { get; set; }

    private MoviesResponse _movies;
    private GenresResponse _genres;

    private string _titleValue;
    private IEnumerable<int> _selectedGenres;
    private int? _yearValue;
    private int? _ratingMin;
    private int? _ratingMax;

    protected override async Task OnInitializedAsync()
    {
        if (!Security.IsAdministrator())
        {
            NavigationManager.NavigateTo("/unauthorized");
        }

        IsLoading = true;
        try
        {
            var movieRequest = new GetAllMoviesRequest()
            {
                Page = 1,
                PageSize = 10
            };
            var moviesResponse = await MovieService.GetMovies(movieRequest);
            _movies = moviesResponse.Data;
            var genresResponse = await MovieService.GetAllGenresAsync();
            _genres = genresResponse.Data;
            IsLoading = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while initializing the MoviesAdminPanel component.");
            ErrorVisible = true;
            Error = ex.Message;
        }
    }

    private async Task LoadMoviesAsync(LoadDataArgs args)
    {
        if (!ValidateRatings())
        {
            return;
        }

        IsLoading = true;

        string formattedSort = string.Empty;
        var sort = args.Sorts.FirstOrDefault();
        if (sort != null)
        {
            formattedSort = $"{(sort.SortOrder == SortOrder.Ascending ? "+" : "-")}{sort.Property}";
        }

        var movieRequest = new GetAllMoviesRequest()
        {
            Page = AdminMoviesGrid.CurrentPage + 1,
            PageSize = AdminMoviesGrid.PageSize,
            Title = _titleValue,
            GenreIds = _selectedGenres,
            Year = _yearValue,
            MinRating = _ratingMin,
            MaxRating = _ratingMax,
            SortBy = formattedSort
        };

        var moviesResponse = await MovieService.GetMovies(movieRequest);
        _movies = moviesResponse.Data;

        IsLoading = false;
    }

    private const string CreateMovieErrorMessage =
        "An error occurred while trying to open the Create new Movie dialog.";

    private const string DeleteMovieErrorMessage = "An error occurred while trying to delete the movie.";
    private const string EditMovieErrorMessage = "An error occurred while trying to update the role.";

    private async Task AddClick()
    {
        try
        {
            await DialogService.OpenAsync<AddMovie>("Add Movie");
            await LoadMovies();
        }
        catch (Exception ex)
        {
            HandleError(ex, CreateMovieErrorMessage);
        }
    }

    private async Task DeleteClick()
    {
        var movieToDelete = GetSelectedMovie();
        if (movieToDelete == null)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Warning", "No movie selected");
            return;
        }

        bool? confirmResult =
            await DialogService.Confirm($"Are you sure you want to delete this movie ({movieToDelete?.Title})?");
        if (confirmResult == false) return;

        var response = await MovieService.DeleteMovie(movieToDelete.Id);
        if (response.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Movie deleted successfully");
            await LoadMovies();
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

    private async Task EditClick()
    {
        try
        {
            var movieToEdit = GetSelectedMovie();
            if (movieToEdit == null) return;


            await DialogService.OpenAsync<EditMovie>("Edit Movie", CreateDialogParameters(movieToEdit.Id));
            await LoadMovies();
        }
        catch (Exception ex)
        {
            HandleError(ex, EditMovieErrorMessage);
        }
    }

    private async Task LoadMovies()
    {
        var movieRequest = CreateMovieRequest();
        ClearSelection();
        var moviesResponse = await MovieService.GetMovies(movieRequest);
        _movies = moviesResponse.Data;
    }

    private GetAllMoviesRequest CreateMovieRequest()
    {
        return new GetAllMoviesRequest
        {
            Page = AdminMoviesGrid.CurrentPage + 1,
            PageSize = AdminMoviesGrid.PageSize,
        };
    }

    private void HandleError(Exception ex, string errorMessage)
    {
        Logger.LogError(ex, errorMessage);
        ErrorVisible = true;
        Error = errorMessage;
    }

    private MovieResponse GetSelectedMovie()
    {
        return _selectedMovie?.FirstOrDefault();
    }

    private async Task DetailsClick()
    {
        var movieToRead = GetSelectedMovie();
        if (movieToRead == null) return;

        await DialogService.OpenAsync<MovieDetails>(
            "Movie Details",
            CreateDialogParameters(movieToRead.Id),
            CreateDialogOptions("800px")
        );
    }

    private Dictionary<string, object> CreateDialogParameters(Guid movieId)
    {
        return new Dictionary<string, object> { { "Id", movieId } };
    }

    private DialogOptions CreateDialogOptions(string width)
    {
        return new DialogOptions
        {
            Width = width,
        };
    }

    private void ClearSelection()
    {
        _selectedMovie = null;
    }

    private void OnSelectedGenresChange(object value)
    {
        if (_selectedGenres != null && !_selectedGenres.Any())
        {
            _selectedGenres = null;
        }
    }

    private bool ValidateRatings()
    {
        ErrorVisible = false;
        if (!_ratingMin.HasValue || !_ratingMax.HasValue)
        {
            return true;
        }

        if (!(_ratingMin > _ratingMax)) return true;
        Error = "Minimum rating cannot be greater than maximum rating.";
        ErrorVisible = true;
        return false;
    }
}