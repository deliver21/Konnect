using Konnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Konnect.Services
{
    public class BlockedUserCheckAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BlockedUserCheckAttribute(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Skip check if user is not authenticated
            if (!user.Identity.IsAuthenticated)
            {
                return;
            }

            var currentUser = await _userManager.GetUserAsync(user);

            // Check if user is blocked
            if (currentUser != null && currentUser.IsBlocked)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { blocked = true });
            }
        }
    }
}
