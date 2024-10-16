using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using MovieHub.Data;

namespace MovieHub.Controllers
{
    public partial class ExportIdentityDbController : ExportController
    {
        private readonly IdentityDbContext context;
        private readonly IdentityDbService service;

        public ExportIdentityDbController(IdentityDbContext context, IdentityDbService service)
        {
            this.service = service;
            this.context = context;
        }
    }
}
