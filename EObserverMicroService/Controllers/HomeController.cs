using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EObserver.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        public HomeController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _hostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        
        public IActionResult Index()
        {
            return View();
        }

    }
}