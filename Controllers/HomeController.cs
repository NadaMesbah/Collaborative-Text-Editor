using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeCollaborativeApp.Data;
using RealTimeCollaborativeApp.Models;
using RealTimeCollaborativeApp.Models.ViewModel;
using System.Diagnostics;
using System.Security.Claims;

namespace RealTimeCollaborativeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger ,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Create()
        {
            //to find the logged in user Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            DocumentVM documentVM = new()
            {
                Documents = _context.Documents.ToList(),
                MaxDocsAllowed = 4,
                UserId= userId,
            };
            return View(documentVM);
        }
        [Authorize]
        public IActionResult TextEditor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            DocumentVM documentVM = new()
            {
                UserId= userId,
                UserName= userName,
            };
            return View(documentVM) ;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}