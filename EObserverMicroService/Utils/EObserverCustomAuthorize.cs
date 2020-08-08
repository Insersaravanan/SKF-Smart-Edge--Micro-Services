using Dapper;
using EMaintanance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Utils
{
    internal class EObserverAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "SKFRoles";

        public EObserverAuthorizeAttribute(string roles) => SKFRoles = roles;

        public string SKFRoles
        {
            get
            {
                return Policy.Substring(POLICY_PREFIX.Length);
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value}";
            }
        }
    }

    internal class EObserverRequirement : IAuthorizationRequirement
    {
        public string BizBookRoles { get; private set; }

        public EObserverRequirement(string roles) { BizBookRoles = roles; }
    }

    internal class EObserverPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "SKFRoles";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public EObserverPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
                policyName.Substring(POLICY_PREFIX.Length).Length > 0)
            {
                var _roles = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new EObserverRequirement(_roles));
                return Task.FromResult(policy.Build());
            }

            // If the policy name doesn't match the format expected by this policy provider,
            // try the fallback provider. If no fallback provider is used, this would return 
            // Task.FromResult<AuthorizationPolicy>(null) instead.
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }

    internal class EObserverAuthorizationHandler : AuthorizationHandler<EObserverRequirement>
    {
        private readonly ILogger<EObserverAuthorizationHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public EObserverAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ILogger<EObserverAuthorizationHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EObserverRequirement requirement)
        {
            try
            {
                //var iName = _httpContextAccessor.HttpContext.User.Identity.Name;
                //var data = JToken.Parse(_httpContextAccessor.HttpContext.Session.GetString(iName + "_GlobalInformation"));
                List<RoleManager> userRoles = null;
                var UserId = _httpContextAccessor.HttpContext.User.FindFirst("UserId")?.Value;

                Utility util = new Utility(_configuration);
                using (var conn = util.MasterCon())
                {
                    var sql1 = @"select distinct ps.ProgramCode, p.PrivilegeCode
                            from Roles as r1
                            join [dbo].[RolePrgPrivilegeRelation] rp ON r1.RoleId = rp.RoleId and rp.Active = 'Y'
                            join [dbo].[Programs] ps ON ps.ProgramId = rp.ProgramId 
                            join [dbo].Privileges p on rp.PrivilegeId = p.PrivilegeID
                            join RoleGroupRoleRelation rr on rr.RoleId = r1.RoleId and rr.Active = 'Y' 
                            join RoleGroup r on r.RoleGroupId = rr.RoleGroupId and r.Active = 'Y'
                            join UserRoleGroupRelation ur on ur.RoleGroupId = r.RoleGroupId and ur.Active = 'Y'
                            WHERE ur.UserId = " + UserId + " order by ps.ProgramCode";

                    userRoles = conn.Query<RoleManager>(sql1).ToList();

                    IDictionary<string, object> dict = new Dictionary<string, object>
                     {
                        { "userInfo", null },
                        { "userRoles", userRoles}
                    };

                }

                var roleList = requirement.BizBookRoles.Split(","); //Split the roles from controller for evaluate it.

                if (userRoles != null && userRoles.Count() > 0)
                {
                    bool validated = false;
                    foreach (var _role in roleList)
                    {
                        //Validate the branchinfo to check the user has specific role
                        if (!validated)
                        {
                            var _roleSplit = _role.Split(":");
                            var _roleName = _roleSplit[0];
                            var _privilegeCode = _roleSplit.Length > 1 ? _roleSplit[1] : null;

                            JArray jRoles = JArray.FromObject(userRoles);

                            if (_privilegeCode != null)
                            {
                                if (jRoles.FirstOrDefault(x => x.Value<string>("ProgramCode") == _roleName && x.Value<string>("PrivilegeCode") == _privilegeCode) != null)
                                {
                                    context.Succeed(requirement);
                                    validated = true;
                                }
                            }
                            else
                            {
                                if (jRoles.FirstOrDefault(x => x.Value<string>("ProgramCode") == _roleName) != null)
                                {
                                    context.Succeed(requirement);
                                    validated = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Task.CompletedTask;
        }
    }

    public class RoleManager
    {
        public string ProgramCode { get; set; }
        public string PrivilegeCode { get; set; }
    }

}
