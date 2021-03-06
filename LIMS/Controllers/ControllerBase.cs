﻿using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace LIMS.Controllers
{
    public class ControllerBase : Controller, IRequestContext
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _dbContext;
        private string _userId;
        private string _username;

        public ApplicationSignInManager SignInManager =>
            _signInManager ?? (_signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public RoleManager<IdentityRole> RoleManager =>
            _roleManager ?? (_roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());

        public ApplicationDbContext DbContext =>
            _dbContext ?? (_dbContext = HttpContext.GetOwinContext().Get<ApplicationDbContext>());

        public string UserId =>
            _userId ?? (_userId = HttpContext.User?.Identity?.GetUserId());

        public string UserName =>
            _username ?? (_username = HttpContext.User?.Identity?.GetUserName());

        public Task LogAsync(string message)
        {
            return LogsDao.Create(this, message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}