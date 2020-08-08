using EMaintanance.Data;
using EMaintanance.UserModels;
using EMaintanance.Utils;
using EMaintananceApi.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Providers
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _serializerSettings;
        private UserDbContext _userDb;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;

        public TokenProviderMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context,
            UserDbContext userDb,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userDb = userDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals("/api/token", StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }


            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            try
            {
                var username = context.Request.Form["username"].ToString();
                var password = context.Request.Form["password"];

                var CheckCredential = "DATABASE"; // DATABASE or STATIC

                if (CheckCredential == "DATABASE")
                {
                    var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
                    if (!result.Succeeded)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid username or password.");
                        return;
                    }
                    var user = await _userManager.Users
                        .SingleAsync(i => i.UserName == username);
                    if (user == null)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid username or password.");
                        return;
                    }

                    var response = GetLoginToken.Execute(_configuration, user, _userDb);

                    // Serialize and return the response
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
                } else
                {
                    if (!(username == "apiadmin" && password == "@dminap!"))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid username or password.");
                        return;
                    }

                    var response = GetLoginToken.ExecuteStaticUser("apiadmin", "@dminap!", _userDb);

                    // Serialize and return the response
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));

                }
            }
            catch (Exception ex)
            {
                //TODO log error
                //Logging.GetLogger("Login").Error("Erorr logging in", ex);
            }
        }

    }
}
