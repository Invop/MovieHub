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
        try
        {
            Logger.LogInformation("Initializing the MoviesAdminPanel component.");
            var movieRequest = new GetAllMoviesRequest()
            {
                Page = 1,
                PageSize = 10
            };
            _movies = await MovieService.GetMovies(movieRequest);
            _genres = await MovieService.GetAllGenres();
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

        _movies = await MovieService.GetMovies(movieRequest);

        IsLoading = false;
    }

    private async Task AddClick()
    {
        try
        {
            Logger.LogInformation("Opening dialog to add a new application role.");
            await DialogService.OpenAsync<AddApplicationRole>("Add Application Role");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while trying to open the add application role dialog.");
            ErrorVisible = true;
            Error = ex.Message;
        }
    }

    private async Task DeleteClick()
    {
        try
        {
            MovieResponse movieToDelete = default;
            if (_selectedMovie?.Count > 0)
            {
                movieToDelete = _selectedMovie.First();
            }

            if (await DialogService.Confirm($"Are you sure you want to delete this movie({movieToDelete?.Title})?") ==
                true)
            {
                if (movieToDelete != null)
                    await MovieService.DeleteMovie(movieToDelete.Id);
                var movieRequest = new GetAllMoviesRequest()
                {
                    Page = AdminMoviesGrid.CurrentPage + 1,
                    PageSize = AdminMoviesGrid.PageSize,
                };
                ClearSelection();
                _movies = await MovieService.GetMovies(movieRequest);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while trying to delete the role.");
            ErrorVisible = true;
            Error = ex.Message;
        }
    }

    private async Task UpdateClick()
    {
        try
        {
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while trying to delete the role.");
            ErrorVisible = true;
            Error = ex.Message;
        }
    }

    private void DetailsClick()
    {
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