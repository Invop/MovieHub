using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MovieHub.Contracts.Requests;
using MovieHub.Services;
using Radzen;

namespace MovieHub.Components.Pages
{
    public partial class Index
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

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

        [Inject]
        protected ILogger<Index> Logger { get; set; } // Inject the logger

        private string Token { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var rateRequest = new RateMovieRequest()
            {
                Rating = 5
            };
            await MovieService.RateMovie(Guid.Parse("a9ce762c-a7be-41c7-a200-4a2493348a77"), rateRequest);
            var request = new GetAllMoviesRequest
            {
                Title = null,
                Year = null,
                SortBy = null,
                Page = 1,
                PageSize = 3
            };

            var movies = await MovieService.GetMovies(request);
            
            foreach (var movie in movies.Items)
            {
                Logger.LogInformation("Movie Title: {Title}, Id: {Id}, Rating: {Rating}", movie.Title,movie.Id, movie.Rating);
            }
        }
    }
}