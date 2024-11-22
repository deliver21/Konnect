using Konnect.Models;
using Konnect.Repository;
using Konnect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Konnect.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserManagementController(IUnitOfWork unit, SignInManager<IdentityUser> signInManager) 
        {
            _unit = unit;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
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
            var userIdNextMove = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var checkUser = _unit.ApplicationUser.Get(u => u.Id == userIdNextMove);
            if (checkUser != null)
            {
                 return checkUser.IsBlocked;
            }
                      
            return true;            
        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {            
            IEnumerable<ApplicationUser> obj = _unit.ApplicationUser.GetAll();
            foreach(ApplicationUser user in obj)
            {
                var formatTime = DateTimeFormat.FormatString(user.LastSeen);
                user.Interval = $"Last seen {Interval.SetInterval(user.LastSeen) }";
                user.LastSeen = DateTime.Parse(formatTime);
            }
            return Json(new { data = obj.OrderByDescending(u=> u.LastSeen) });
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] List<string> userIds)
        {
            if (IsUserBlockedOrDeleted())
            {
                await _signInManager.SignOutAsync();
                return Unauthorized(new { message = "Your account is blocked." });
            }
            if (userIds == null || !userIds.Any())
            {
                return BadRequest(new { message = "No users selected for deletion." });
            }
            try
            {
                foreach (var userId in userIds)
                {
                    var user = _unit.ApplicationUser.Get(u => u.Id == userId);
                    if (user != null)
                    {
                        _unit.ApplicationUser.Remove(user);
                    }
                }
                _unit.Save();
                return Ok(new { message = $"{userIds.Count} user(s) deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error occurred during deletion.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkLock([FromBody] List<string> ids)
        {
            if (IsUserBlockedOrDeleted())
            {
                await _signInManager.SignOutAsync();
                return Unauthorized(new { message = "Your account is blocked." });
            }
            foreach (var id in ids)
            {
                var user = _unit.ApplicationUser.Get(u => u.Id == id,false);
                if (user != null && !user.IsBlocked)
                {
                    user.IsBlocked = true; // Set blocked flag
                    _unit.ApplicationUser.Update(user);
                }
            }
            _unit.Save();
            return Json(new { message = "Selected users have been blocked." });
        }

        [HttpPost]
        public async Task<IActionResult> BulkUnlock([FromBody] List<string> ids)
        {
            if (IsUserBlockedOrDeleted())
            {
                await _signInManager.SignOutAsync();
                return Unauthorized(new { message = "Your account is blocked." });
            }
            foreach (var id in ids)
            {
                var user = _unit.ApplicationUser.Get(u => u.Id == id);
                if (user != null && user.IsBlocked)
                {
                    user.IsBlocked = false; // Remove blocked flag
                    _unit.ApplicationUser.Update(user);
                }
            }
            _unit.Save();
            return Json(new { message = "Selected users have been unlocked." });
        }
        #endregion
    }
}
