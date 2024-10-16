using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Server.Models;

namespace MovieHub.Server.Controllers;

[Route("Account/[action]")]
public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _env = env;
        _configuration = configuration;
    }

    private IActionResult RedirectWithError(string error, string redirectUrl = null)
    {
        if (!string.IsNullOrEmpty(redirectUrl))
            return Redirect($"~/Login?error={error}&redirectUrl={Uri.EscapeDataString(redirectUrl.Replace("~", ""))}");
        return Redirect($"~/Login?error={error}");
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        if (returnUrl != "/" && !string.IsNullOrEmpty(returnUrl))
            return Redirect($"~/Login?redirectUrl={Uri.EscapeDataString(returnUrl)}");

        return Redirect("~/Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password, string redirectUrl)
    {
        redirectUrl = string.IsNullOrEmpty(redirectUrl) ? "~/" :
            redirectUrl.StartsWith("/") ? redirectUrl : $"~/{redirectUrl}";

        if (_env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "admin"),
                new(ClaimTypes.Email, "admin")
            };

            _roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
            await _signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = userName, Email = userName },
                false, claims);

            return Redirect(redirectUrl);
        }

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);

            if (result.Succeeded) return Redirect(redirectUrl);
        }

        return RedirectWithError("Invalid user or password", redirectUrl);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            return BadRequest("Invalid password");

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(id);
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (result.Succeeded) return Ok();

        var message = string.Join(", ", result.Errors.Select(error => error.Description));

        return BadRequest(message);
    }

    [HttpPost]
    public ApplicationAuthenticationState CurrentUser()
    {
        return new ApplicationAuthenticationState
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            Name = User.Identity.Name,
            Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
        };
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return Redirect("~/");
    }
}