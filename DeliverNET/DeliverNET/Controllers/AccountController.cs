using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DeliverNET.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using DeliverNET.Models.AccountViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using DeliverNET.Infrastructure.Account;
using DeliverNET.Managers.Interfaces;

namespace DeliverNET.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<DeliverNETUser> _userManager;
        private readonly SignInManager<DeliverNETUser> _signInManager;
        private readonly ILogger _logger;
        private readonly DeliverNETClaimManager _claimManager;
        private readonly IMasterManager _masterManager;
        private string ErrorMessage;

        public AccountController(
            UserManager<DeliverNETUser> userManager,
            SignInManager<DeliverNETUser> signInManager,
            ILogger<AccountController> logger,
            DeliverNETClaimManager claimManager,
            IMasterManager masterManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _claimManager = claimManager;
            _masterManager = masterManager;
        }

        #region"Login/Logout"
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ExternalLogins"] = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // TODO Change "lockout on failure" when implemented
                // TODO Clarify username login
                var result = await _signInManager.PasswordSignInAsync(model.Email.Split('@')[0], model.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {0} logged in.", model.Email);
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    return RedirectOnJobType(user, "~/Account/Login");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User locked out.");
                    // TODO Create a lockout view!
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                    return View(model);
                }
            }
            return View(model);
        }

        // TODO Facebook login is not fully implemented
        #region "External Login Facebook"
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View(nameof(AccountController.Login));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", returnUrl);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(AccountController.Login), new { returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading user information from external provider.";
                return RedirectToAction(nameof(AccountController.Login), new { returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(AccountController.Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    ExternalLoginViewModel Input = new ExternalLoginViewModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return View("ExternalLogin");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new DeliverNETUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["LoginProvider"] = info.LoginProvider;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region "Register"
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new DeliverNETUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = model.DOB,
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add claims
                    switch (model.JobType)
                    {
                        case JobTypes.Individual:
                            _masterManager.GetDelivererManager().Create(user);
                            await _claimManager.AddClaim(user, JobTypes.Individual);
                            break;
                        case JobTypes.Businessman:
                            string newUserId = _userManager.FindByEmailAsync(user.Email).Result.Id;
                            _masterManager.GetBusinessOwnerManager().Create(user.Id);
                            await _claimManager.AddClaim(user, JobTypes.Businessman);
                            break;
                        case JobTypes.Cashier:
                            await _claimManager.AddClaim(user, JobTypes.Cashier);
                            break;
                        default:
                            break;
                    }




                    _logger.LogInformation("User created new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User logged in.");

                    return RedirectOnJobType(user, "~/Account/Register");
                }
                AddErrors(result);
            }

            // If we got this far, something went bad, redisplay form.
            return View(model);
        }
        #endregion



        #region "Helpers"
        private void AddErrors(IdentityResult result)
        {
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private IActionResult RedirectOnJobType(DeliverNETUser user, string returnUrl)
        {
            // TODO Remove if you understand async!
            if (user == null)
                return RedirectToLocal(returnUrl);

            // Decide if deliverer or business and redirect acoordingly
            if (
                _claimManager.HasClaim(user, JobTypes.Individual).Result
                )
                return RedirectToAction("IndexIndi", "ProfileIndi");
            else if (
                _claimManager.HasClaim(user, JobTypes.Businessman).Result ||
                _claimManager.HasClaim(user, JobTypes.Cashier).Result
                )
                return RedirectToAction("IndexBusi", "ProfileBusi");
            else
                return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}