using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace MovieHub.Server.Models;

public class ApplicationRole : IdentityRole
{
    [JsonIgnore] public ICollection<ApplicationUser> Users { get; set; }
}