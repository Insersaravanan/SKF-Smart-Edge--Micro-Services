using EMaintanance.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Areas.Identity
{

    //public class ApplicationSignInManager : SignInManager<IdentityUser>
    //{
    //    public ApplicationSignInManager(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor,
    //             IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
    //             ILogger<SignInManager<IdentityUser>> logger)
    //             : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger) { }

    //    public override Task<bool> CanSignInAsync(IdentityUser user)
    //    {
    //        if (!user.LockoutEnabled)
    //            return false;

    //        return base.CanSignInAsync(user);
    //    }
    //}
}
