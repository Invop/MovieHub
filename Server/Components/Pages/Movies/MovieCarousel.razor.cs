using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MovieHub.Components.Pages.Movies.AdminPanel;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Requests.Movie;
using MovieHub.Contracts.Responses;
using MovieHub.Contracts.Responses.Movie;
using MovieHub.Services;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Components.Pages.Movies;

public partial class MovieCarousel
{
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }

    [Inject] protected ILogger<MoviesAdminPanel> Logger { get; set; }
    [Inject] protected SecurityService Security { get; set; }

    [Inject] protected MovieService MovieService { get; set; }
    protected bool IsLoading = false;
    RadzenCarousel carousel;

    protected MoviesResponse Movies;
    private static readonly Random _random = new Random();
    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        var request = new GetAllMoviesRequest
        {
            SortBy = RandomSortBy()
        };
        var moviesResponse = await MovieService.GetMovies(request);
        Movies = moviesResponse.Data;
        IsLoading = false;
    }
    private static string RandomSortBy()
    {
        var sortOptions = new[] { "Title", "YearOfRelease", "Rating" };
        return sortOptions[_random.Next(sortOptions.Length)];
    }
}