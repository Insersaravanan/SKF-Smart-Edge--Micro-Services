using Dapper;
using EMaintanance.Models;
using EMaintanance.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EMaintanance.Utils
{
    public class CurrentUser
    {
        private readonly HttpContext _httpContext;
        private readonly IConfiguration _configuration;

        public int UserId { get; set; }
        public string Email { get; set; }
        public string SessionId { get; set; }
        public string HostIP { get; set; }
        public string AppSession { get; set; }
        public string HeaderHost { get; set; }
        public string HeaderReferer { get; set; }

        public CurrentUser(HttpContext httpContext)
        {
            _httpContext = httpContext;
            Email = _httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            UserId = Int32.Parse(_httpContext.User.FindFirst("UserId")?.Value);
        }

        public CurrentUser(HttpContext httpContext, IConfiguration configuration)
        {
            _httpContext = httpContext;
            _configuration = configuration;
            Email = _httpContext.User.Identity.Name;
            HostIP = _httpContext.Connection.RemoteIpAddress.ToString();
            AppSession = _httpContext.Request.Cookies[".AspNetCore.Identity.Application"];//[".AspNetCore.Session"];
            if (AppSession != null)
            {
                SessionId = _httpContext.Session.Id + "---" + AppSession;
            }
            else
            {
                SessionId = _httpContext.Session.Id;
            }
            HeaderHost = _httpContext.Request.Host.Host;
            //StringValues tempReferer = "";
            //HeaderReferer = _httpContext.Request.Headers.("HeaderReferer"); //TryGetValue("HeaderReferer", out tempReferer);

            if (HostIP != null && (HostIP == "" || HostIP == "::1"))
            {
                HostIP = _httpContext.Connection.LocalIpAddress.ToString();
            }

            try
            {
                VerifySession();
                var data = JToken.Parse(_httpContext.Session.GetString(Email + "_GlobalInformation"));
                UserId = (int)data["userInfo"]["userId"];
            }
            catch (Exception ex)
            {
                throw new CustomException("Error while capturing Logged In User Info.", "Error", true, ex);
            }

        }

        private bool VerifySession()
        {
            var iName = _httpContext.User.Identity.Name;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (_httpContext.Session.GetString(iName + "_GlobalInformation") == null)
            {
                Utility util = new Utility(_configuration);
                using (var conn = util.MasterCon())
                {
                    var sql1 = "Select UserId as userId from Users where EmailId ='" + iName + "'";
                    var userInfo = conn.QueryFirstOrDefault<Users>(sql1);

                    sql1 = @"select distinct ps.ProgramCode, p.PrivilegeCode
                            from Roles as r1
                            join [dbo].[RolePrgPrivilegeRelation] rp ON r1.RoleId = rp.RoleId and rp.Active = 'Y'
                            join [dbo].[Programs] ps ON ps.ProgramId = rp.ProgramId 
                            join [dbo].Privileges p on rp.PrivilegeId = p.PrivilegeID
                            join RoleGroupRoleRelation rr on rr.RoleId = r1.RoleId and rr.Active = 'Y' 
                            join RoleGroup r on r.RoleGroupId = rr.RoleGroupId and r.Active = 'Y'
                            join UserRoleGroupRelation ur on ur.RoleGroupId = r.RoleGroupId and ur.Active = 'Y'
                            WHERE ur.UserId = " + userInfo.UserId + " order by ps.ProgramCode";

                    var userRoles = conn.Query<dynamic>(sql1).ToList();

                    IDictionary<string, object> dict = new Dictionary<string, object>
                     {
                        { "userInfo", userInfo },
                        { "userRoles", userRoles}
                    };

                    _httpContext.Session.SetString((iName + "_GlobalInformation"), JsonConvert.SerializeObject(dict, settings));
                }
            }

            return true;
        }

    }
}
