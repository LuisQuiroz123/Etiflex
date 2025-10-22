using EtiWebUsers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EtiWebUsers.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // Total de usuarios
            var totalUsers = _userManager.Users.Count();

            // Contar usuarios por rol de manera secuencial
            int superAdminsCount = 0;
            int adminsCount = 0;
            int usersCount = 0;

            if (await _roleManager.RoleExistsAsync("SuperAdmin"))
                superAdminsCount = (await _userManager.GetUsersInRoleAsync("SuperAdmin")).Count;

            if (await _roleManager.RoleExistsAsync("Admin"))
                adminsCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;

            if (await _roleManager.RoleExistsAsync("User"))
                usersCount = (await _userManager.GetUsersInRoleAsync("User")).Count;

            // Pasar los datos a la vista con ViewBag
            ViewBag.TotalUsers = totalUsers;
            ViewBag.SuperAdmins = superAdminsCount;
            ViewBag.Admins = adminsCount;
            ViewBag.Users = usersCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
