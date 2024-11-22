using Konnect.Models;
using Konnect.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Konnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unit;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,IUnitOfWork unit)
        {
            _logger = logger;
            _signInManager = signInManager;
            _unit = unit;
        }
        public async Task <IActionResult> Index()
        {
            if (IsUserBlockedOrDeleted())
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            return View();
        }

        private bool IsUserBlockedOrDeleted()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity.Claims.Count() == 0) return true;
            var userIdNextMove = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var checkUser = _unit.ApplicationUser.Get(u => u.Id == userIdNextMove);
            if (checkUser != null)
            {
                return checkUser.IsBlocked;
            }

            return true;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}