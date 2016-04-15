﻿using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using KeyHub.Model;
using Microsoft.Web.WebPages.OAuth;
using KeyHub.Data;
using KeyHub.Web.Models;
using KeyHub.Web.ViewModels.User;
using MvcFlash.Core;
using WebMatrix.WebData;
using System.Data.Entity.Core;
using Microsoft.AspNet.Identity;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly UserManager<User, int> userManager;
        public AccountController(IDataContextFactory dataContextFactory, UserManager<User, int> userManager)
        {
            this.dataContextFactory = dataContextFactory;
            this.userManager = userManager;
        }

        /// <summary>
        /// Get list of users
        /// </summary>
        /// <returns>User index view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                // Eager loading users (except current user) and roles
                var usersQuery = (from u in context.Users where u.Email != User.Identity.Name select u)
                                 .Include(u => u.Rights.Select(r => r.RightObject))
                                 .OrderBy(u => u.Email);

                var viewModel = new UserIndexViewModel(context.GetUser(HttpContext.User.Identity), usersQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single User
        /// </summary>
        /// <returns>Create User view</returns>
        [Authorize(Roles = Role.SystemAdmin)]
        public ActionResult Create()
        {
            var viewModel = new UserCreateViewModel(thisOne: true);
            return View(viewModel);
        }

        /// <summary>
        /// Save created User into context and redirect to index
        /// </summary>
        /// <param name="viewModel">Created UserViewModel</param>
        /// <returns>Redirectaction to index if successful</returns>
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Role.SystemAdmin)]
        public ActionResult Create(UserCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user

                var user = new User() { Email = viewModel.User.Email };
                var result = userManager.Create(user, viewModel.User.Password);
                if (result.Succeeded)
                {
                    Flash.Success("New user succesfully created");
                    if (Url.IsLocalUrl(viewModel.RedirectUrl))
                    {
                        return Redirect(viewModel.RedirectUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    result
                        .Errors
                        .ToList()
                        .ForEach(error => ModelState.AddModelError("", error));
                    var newViewModel = new UserCreateViewModel(thisOne: true);
                    return View(viewModel);

                }
            }

            //Viewmodel invalid, recall create
            return Create();
        }

        /// <summary>
        /// Edit a single User
        /// </summary>
        /// <param name="id">Id if the user to edit</param>
        /// <returns>Edit User view</returns>
        public ActionResult Edit(int id)
        {
            using (var context = dataContextFactory.Create())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);

                if (user == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                if (!User.IsInRole(Role.SystemAdmin) && user.Email != User.Identity.Name)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                var viewModel = new UserEditViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email
                };

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited User into context and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited UserViewModel</param>
        /// <returns>Redirectaction to index if successful</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.Create())
                {
                    var user = context.Users.FirstOrDefault(x => x.Id == viewModel.UserId);

                    if (user == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                    if (!User.IsInRole(Role.SystemAdmin) && user.Email != User.Identity.Name)
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                    //Email can always be updated
                    user.Email = viewModel.Email;
                    context.SaveChanges();
                    userManager.

                    return RedirectToAction("Index");
                }
            }

            return Edit(viewModel.UserId);
        }

        /// <summary>
        /// Login to KeyHub
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = (!String.IsNullOrEmpty(returnUrl)) ? returnUrl : Url.Action("Index", "Home", null, "http");
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var dataContext = dataContextFactory.Create())
                {
                    var user = dataContext.Users.Where(u => u.Email == model.Email).SingleOrDefault();

                    if (user != null)
                    {
                        if (WebSecurity.Login(user.Email, model.Password, persistCookie: model.RememberMe))
                        {
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }

                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Log off
        /// </summary>
        /// <returns>Redirect to home</returns>
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="returnUrl">Url to return to upon successfull registration</param>
        /// <returns>Register user view</returns>
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Register user and redirect to return URL if specified
        /// </summary>
        /// <param name="model">RegisterViewModel for new user</param>
        /// <param name="returnUrl">Url to redirect to after successfull registration</param>
        /// <returns>Redirect to Index, or ReturnUrl if specified</returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var newMembershipUserIdentifier = Guid.NewGuid().ToString();

                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(newMembershipUserIdentifier, model.Password, new { Email = model.Email });
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("IX_Email") && exception.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError("",
                            "The email address registered is already in use on this site using a different login method.  "
                            + "Please login with the original login method used for that email.  "
                            + "Then you may associate other login methods with your account.  ");

                        return View(model);
                    }
                    throw;
                }

                if (WebSecurity.Login(newMembershipUserIdentifier, model.Password))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Failed to create a user with the provided email and password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <returns>Change password view</returns>
        public ActionResult ChangePassword()
        {
            using (var dataContext = dataContextFactory.Create())
            {
                var viewModel = new ChangePasswordViewModel(dataContext.GetUser(User.Identity).Email);
                return View(viewModel);
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="model">Changed password model</param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    Flash.Success("Your password has been changed.");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region OpenAuth
        /// <summary>
        /// Get a list of external logins
        /// </summary>
        /// <param name="returnUrl">Return url to go to upon successfull login</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        /// <summary>
        /// Login from external
        /// </summary>
        /// <param name="provider">Provider to login with</param>
        /// <param name="returnUrl">Url to go to upon successfull login</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// Handle external login from OpenID provider
        /// </summary>
        /// <param name="returnUrl">Url to go to upon successfull login</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            //Get result from OpenID provider
            AuthenticationResult authenticationResult = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!authenticationResult.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            //Login with authentication result
            if (OAuthWebSecurity.Login(authenticationResult.Provider, authenticationResult.ProviderUserId, createPersistentCookie: true))
            {
                return RedirectTo(returnUrl);
            }

            var userName = OAuthWebSecurity.GetUserName(authenticationResult.Provider, authenticationResult.ProviderUserId);
            var loginData = OAuthWebSecurity.SerializeProviderUserId(authenticationResult.Provider, authenticationResult.ProviderUserId);
            var displayName = OAuthWebSecurity.GetOAuthClientData(authenticationResult.Provider).DisplayName;

            // If the current user is logged in add the new account
            if (User.Identity.IsAuthenticated)
            {
                OAuthWebSecurity.CreateOrUpdateAccount(authenticationResult.Provider, authenticationResult.ProviderUserId, User.Identity.Name);
                return RedirectTo(returnUrl);
            }

            var membershipUserIdentifier = Guid.NewGuid().ToString();

            try
            {
                // Insert a new user into the database
                using (var db = dataContextFactory.Create())
                {
                    // Insert name into the profile table
                    db.Users.Add(new User { Email = authenticationResult.UserName });
                    db.SaveChanges();
                }
            }
            catch (DbUpdateException e)
            {
                var innerException1 = e.InnerException as UpdateException;
                if (innerException1 == null)
                    throw;

                var innerException2 = innerException1.InnerException as SqlException;
                if (innerException2 == null)
                    throw;

                var innerExceptionMessage = innerException2.Message ?? "";

                if (innerExceptionMessage.Contains("IX_Email") && innerExceptionMessage.Contains("duplicate"))
                {
                    Flash.Error("The email address used to login is already in use on this site using a different login method.  "
                        + "Please login with the original login method used for that email.  "
                        + "Then you may associate other login methods with your account.  ");

                    return RedirectToAction("Login");
                }
                else
                {
                    throw;
                }
            }

            OAuthWebSecurity.CreateOrUpdateAccount(authenticationResult.Provider, authenticationResult.ProviderUserId, membershipUserIdentifier);
            OAuthWebSecurity.Login(authenticationResult.Provider, authenticationResult.ProviderUserId, createPersistentCookie: true);

            return RedirectTo(returnUrl);
        }

        /// <summary>
        /// Show login failure
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult LinkAccount()
        {
            using (var context = dataContextFactory.Create())
            {
                var model = LinkAccountModel.ForUser(context, User.Identity);
                return View("LinkAccount", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkAccount(string provider)
        {
            return new ExternalLoginResult(provider, Url.Action("LinkAccountCallback"));
        }

        public ActionResult LinkAccountCallback()
        {
            AuthenticationResult authenticationResult = OAuthWebSecurity.VerifyAuthentication(Url.Action("LinkAccountCallback"));
            if (!authenticationResult.IsSuccessful)
            {
                Flash.Error("The account was unable to be linked.");
                return RedirectToAction("LinkAccount");
            }

            if (OAuthWebSecurity.Login(authenticationResult.Provider, authenticationResult.ProviderUserId, createPersistentCookie: true))
            {
                Flash.Success("Your " + authenticationResult.Provider + " login was already linked.");
                return RedirectToAction("LinkAccount");
            }

            OAuthWebSecurity.CreateOrUpdateAccount(authenticationResult.Provider, authenticationResult.ProviderUserId, User.Identity.Name);
            Flash.Success("Your " + authenticationResult.Provider + " login has been linked.");
            return RedirectToAction("LinkAccount");
        }

        public class UnlinkLoginModel
        {
            public string Provider { get; set; }
        }

        public ActionResult UnlinkLogin(string provider)
        {
            return View(new UnlinkLoginModel() { Provider = provider });
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("UnlinkLogin")]
        public ActionResult UnlinkLogin_Post(string provider)
        {
            using (var context = dataContextFactory.Create())
            {
                var model = LinkAccountModel.ForUser(context, User.Identity);

                if (!model.AllowRemovingLogin)
                {
                    Flash.Error(
                        "The login could not be unlinked because it is the last login available for this account.");
                }
                else
                {
                    var providerAccount = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name)
                        .Single(a => a.Provider.ToLower() == provider.ToLower());

                    if (OAuthWebSecurity.DeleteAccount(providerAccount.Provider, providerAccount.ProviderUserId))
                    {
                        Flash.Success("Your " + provider + " login has been unlinked");
                    }
                    else
                    {
                        Flash.Error("The account could not be unlinked.");
                    }
                }
            }

            return RedirectToAction("LinkAccount");
        }

        /// <summary>
        /// Redirect to url or home
        /// </summary>
        /// <param name="url">Url to redirect to</param>
        /// <returns></returns>
        private ActionResult RedirectTo(string url)
        {
            if (Url.IsLocalUrl(url))
            {
                return Redirect(url);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }
        #endregion
    }
}
