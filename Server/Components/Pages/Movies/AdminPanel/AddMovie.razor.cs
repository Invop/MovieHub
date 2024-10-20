using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages.Movies.AdminPanel;

public partial class AddMovie : ComponentBase
{
    [Inject] protected IJSRuntime JsRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }
    [Inject] protected MovieService MovieService { get; set; }
    [Inject] protected ILogger<AddMovie> Logger { get; set; }
    private bool ErrorVisible { get; set; }
    private string Error { get; set; } = "";
    private MovieRecord Movie { get; set; } = new();
    private GenresResponse Genres { get; set; }
    private bool _isLoading = true;

    private string fileName;
    private long? fileSize;
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
    {
        var genresResponse = await MovieService.GetAllGenresAsync();
        Genres = genresResponse.Data;
        _isLoading = false;
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
    private async Task FormSubmit()
    {
        try
        {

            var request = new CreateMovieRequest()
            {
                Title = Movie.Title,
                YearOfRelease = Movie.YearOfRelease,
                Overview = Movie.Overview,
                PosterBase64 = Movie.PosterBase64,
                Genres = Movie.Genres,
            };
            
            await MovieService.CreateMovie(request);
            NotificationService.Notify(NotificationSeverity.Success, "Success", "Movie created successfully");
            DialogService.Close(null);
        }
        catch (Exception ex)
        {
            ErrorVisible = true;
            Error = ex.Message;
        }
    }

    private void CancelClick()
    {
        DialogService.Close(null);
    }

    void OnChange(string value)
    {
        if (fileName?.Length > MaxFileNameLength)
        {
            OnError(new UploadErrorEventArgs { Message = $"File name must be {MaxFileNameLength} characters or less" });
            Movie.PosterBase64 = null;
            return;
        }
        var posterBase64 = Movie.PosterBase64;
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
        public string Title { get; set; }

        public int YearOfRelease { get; set; }

        public string Overview { get; set; }

        public string PosterBase64 { get; set; }

        public IEnumerable<int> Genres { get; set; } = Enumerable.Empty<int>();
    }
}