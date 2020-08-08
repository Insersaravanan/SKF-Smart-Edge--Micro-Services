using System;
using System.Threading.Tasks;
using EMaintanance.Repository;
using EMaintanance.UserModels;
using EMaintanance.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EObserver.Controllers
{
    [EObserverAuthorize("PRG02")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersRepo uRepo;
        private readonly UserManager<ApplicationUser> _identityManager;
        private IConfiguration _configuration;
        public UsersController(IConfiguration configuration, UserManager<ApplicationUser> identityManager)
        {
            uRepo = new UsersRepo(configuration);
            _identityManager = identityManager;
            _configuration = configuration;
        }

        // GET /GetAllUsers
        [HttpGet]
        [EObserverAuthorize("PRG02:P1")]
        [Route("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await uRepo.GetAllUsers());
        }

        // GET /GetUser/5
        [HttpGet]
        [EObserverAuthorize("PRG02:P1")]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(await uRepo.GetUser(id));
        }


        // GET /GetOrganization/5
        [HttpGet]
        [EObserverAuthorize("PRG02:P1")]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetOrganization(int id)
        {
            return Ok(await uRepo.GetOrganization(id));
        }

    }
}
