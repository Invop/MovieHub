using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MovieHub.Contracts.Responses;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages.Movies;

public partial class MovieDetails : ComponentBase
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
    
    [Inject]
    protected SecurityService Security { get; set; }
    [Inject]
    protected MovieService MovieService { get; set; }
    [Parameter]
    public Guid Id { get; set; }

    private const bool ShowClose = true;
    private bool _isLoading = true;
    
    MovieResponse _movieData;
    protected override async Task OnInitializedAsync()
    {
        _movieData = await MovieService.GetMovie(Id);
        _isLoading = false;
    }
}