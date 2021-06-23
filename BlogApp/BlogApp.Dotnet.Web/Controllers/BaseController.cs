using BlogApp.Dotnet.ApplicationCore.Interfaces;
using BlogApp.Dotnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Dotnet.Web.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {
        public async Task<bool> IsAuthorized(string userID, IAuthorizationService _authorizationService)
        {
            AuthorizationResult result = await _authorizationService.AuthorizeAsync(HttpContext.User, userID, "SameOwnerPolicy");

            return result.Succeeded || HttpContext.User.IsInRole("Administrator");
        }
    }
}
