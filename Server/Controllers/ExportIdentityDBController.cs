using MovieHub.Server.Data;
using MovieHub.Server.Services;

namespace MovieHub.Server.Controllers;

public class ExportIdentityDbController : ExportController
{
    private readonly IdentityDbContext _context;
    private readonly IdentityDbService _service;

    public ExportIdentityDbController(IdentityDbContext context, IdentityDbService service)
    {
        _service = service;
        _context = context;
    }
}