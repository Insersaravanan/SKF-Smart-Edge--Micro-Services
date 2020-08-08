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
    public class RefreshTokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _serializerSettings;
        private UserDbContext _userDb;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;

        public RefreshTokenProviderMiddleware(
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
            if (!context.Request.Path.Equals("/api/refresh", StringComparison.Ordinal))
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
            var refreshToken = context.Request.Form["refreshToken"].ToString();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User must relogin.");
                return;
            }

            var refreshTokenModel = _userDb.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefault(i => i.Token == refreshToken);

            if (refreshTokenModel == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User must relogin.");
                return;
            }

            if (!await _signInManager.CanSignInAsync(refreshTokenModel.User))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User is unable to login.");
                return;
            }

            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(refreshTokenModel.User))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User is locked out.");
                return;
            }

            var user = refreshTokenModel.User;
            var token = GetLoginToken.Execute(_configuration, user, _userDb, refreshTokenModel);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(token, _serializerSettings));
        }
    }
}