using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EMaintanance.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        public AccessDeniedModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString(HttpContext.User.Identity.Name + "_GlobalInformation") == null)
            {
                HttpContext.Response.StatusCode = 302;
                _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                _logger.LogInformation("Session Expired redirect to login page.");
                Response.Redirect("/Identity/Account/Login");
            }
            else
            {
                HttpContext.Response.StatusCode = 303;
            }

            
        }
    }
}

