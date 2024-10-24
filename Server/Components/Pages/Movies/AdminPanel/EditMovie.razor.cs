using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Requests.Movie;
using MovieHub.Contracts.Responses;
using MovieHub.Contracts.Responses.Genre;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages.Movies.AdminPanel;

public partial class EditMovie : ComponentBase
{
    [Parameter] public Guid Id { get; set; }

    [Inject] protected IJSRuntime JsRuntime { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected DialogService DialogService { get; set; }
    [Inject] protected TooltipService TooltipService { get; set; }
    [Inject] protected ContextMenuService ContextMenuService { get; set; }
    [Inject] protected NotificationService NotificationService { get; set; }
    [Inject] protected MovieService MovieService { get; set; }
    [Inject] protected ILogger<EditMovie> Logger { get; set; }
    [Inject] protected SecurityService Security{get;set;}
    private bool ErrorVisible { get; set; }
    private string Error { get; set; } = "";
    private MovieRecord Movie { get; set; } = new();
    private GenresResponse Genres { get; set; }
    
    private bool _isLoading = true;

    private string _fileName;
    private long? _fileSize;
    private const int MaxFileNameLength = 12;
    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const int MaxWidth = 300;
    private const int MaxHeight = 450;

    private static readonly string[] Base64Prefixes =
    [
        "data:image/png;base64,",
        "data:image/jpeg;base64,",
        "data:image/bmp;base64,",
        "data:image/webp;base64,"
    ];

    protected override async Task OnInitializedAsync()
    {        if (!Security.IsAdministrator())
        {
            NavigationManager.NavigateTo("/unauthorized");
        }
        try
        {
            var movieResponse = await MovieService.GetMovie(Id);
            var movieDetails = movieResponse.Data;
            Movie = new MovieRecord()
            {
                Id = movieDetails.Id,
                Title = movieDetails.Title,
                YearOfRelease = movieDetails.YearOfRelease,
                Overview = movieDetails.Overview,
                PosterBase64 = movieDetails.PosterBase64,
                Genres = movieDetails.Genres.Select(g => Genres.Items.FirstOrDefault(x => x.Name == g)?.Id ?? 0)
            };
            var genresResponse = await MovieService.GetAllGenresAsync();
            Genres = genresResponse.Data;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading movie details");
            Error = "Failed to load movie details";
            ErrorVisible = true;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void RemoveBase64Prefix(ref string base64String)
    {
        if (string.IsNullOrEmpty(base64String)) return;

        foreach (var prefix in Base64Prefixes)
        {
            if (base64String.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                base64String = base64String.Substring(prefix.Length);
                break;
            }
        }
    }

    private async Task FormSubmit(MovieRecord movie)
    {
        UpdateMovieRequest request = new UpdateMovieRequest()
        {
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Overview = movie.Overview,
            PosterBase64 = movie.PosterBase64,
            Genres = movie.Genres
        };
    
        var response = await MovieService.UpdateMovie(Id, request);
        if (response.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Movie updated successfully");
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

    private void CancelClick()
    {
        DialogService.Close(null);
    }

    void OnChange(string value)
    {
        if (_fileName?.Length > MaxFileNameLength)
        {
            OnError(new UploadErrorEventArgs { Message = $"File name must be {MaxFileNameLength} characters or less" });
            Movie.PosterBase64 = null;
            return;
        }

        var posterBase64 = value;
        RemoveBase64Prefix(ref posterBase64);
        Movie.PosterBase64 = posterBase64;
        StateHasChanged();
    }

    void OnError(UploadErrorEventArgs args)
    {
        Error = args.Message;
        ErrorVisible = true;
        StateHasChanged();
    }

    private class MovieRecord
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public string Overview { get; set; }
        public string PosterBase64 { get; set; }
        public IEnumerable<int> Genres { get; set; } = Enumerable.Empty<int>();
    }
}