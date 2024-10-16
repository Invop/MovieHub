using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MovieHub.Client.Services;
using Radzen;
using Radzen.Blazor;

namespace MovieHub.Client.Pages
{
    public partial class Login
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

        protected string RedirectUrl;
        protected string Error;
        protected string Info;
        protected bool ErrorVisible;
        protected bool InfoVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);

            Error = query.Get("error");

            Info = query.Get("info");

            RedirectUrl = query.Get("redirectUrl");

            ErrorVisible = !string.IsNullOrEmpty(Error);

            InfoVisible = !string.IsNullOrEmpty(Info);
        }
    }
}