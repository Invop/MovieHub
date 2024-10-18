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


    }
}