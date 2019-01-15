using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Comms.Hubs;
using DeliverNET.Data;
using DeliverNET.Infrastructure.Account;
using DeliverNET.Managers;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models.AccountViewModels;
using DeliverNET.Models.ProfileBusiViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DeliverNET.Controllers
{
    [Authorize(Policy = "BusiOnly")]
    public class ProfileBusiController : Controller
    {
        private readonly UserManager<DeliverNETUser> _userManager;
        private readonly IMasterManager _masterManager;
        private readonly DeliverNETClaimManager _claimManager;

        public ProfileBusiController(
            UserManager<DeliverNETUser> userManager,
            IMasterManager masterManager,
            DeliverNETClaimManager claimManager
            )
        {
            _userManager = userManager;
            _masterManager = masterManager;
            _claimManager = claimManager;
        }

        // TODO Authorize only business cashiers
        [HttpGet]
        public IActionResult IndexBusi()
        {
            DeliverNETUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            Business business;
            if (User.HasClaim(c => c.Value == "Cashier"))
            {
                BusinessCashier cashier = _masterManager.GetBusinessCashierManager().Get(user.Id);
                business = _masterManager.GetBusinessManager().Get(cashier.BusinessId);
            }
            else
            {
                BusinessOwner owner = _masterManager.GetBusinessOwnerManager().Get(user.Id);
                business = _masterManager.GetBusinessManager().Get(owner.BusinessId);
            }

            ViewData["BusinessTitle"] = business.Title;
            return View();
        }

        // TODO Authorize only business owners
        [HttpGet]
        public IActionResult DashboardBusi()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SettingsBusi()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SettingsChangePassword(SettingsBusiChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                DeliverNETUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);

                if (result == PasswordVerificationResult.Success)
                {
                    var resultChange = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (resultChange.Succeeded)
                    {
                        return RedirectToAction("SettingsBusi", "ProfileBusi");
                    }

                    ModelState.AddModelError(string.Empty, "Password change failed.");
                    return View("SettignsBusi");
                }
            }
            return View("SettingsBusi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingsAddCashier(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.JobType = JobTypes.Cashier;
                var user = new DeliverNETUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = model.DOB,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                };
                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {

                    // Add claims
                    switch (model.JobType)
                    {
                        case JobTypes.Individual:
                            _claimManager.AddClaim(user, JobTypes.Individual).Wait();
                            break;
                        case JobTypes.Businessman:
                            _claimManager.AddClaim(user, JobTypes.Businessman).Wait();
                            break;
                        case JobTypes.Cashier:
                            _claimManager.AddClaim(user, JobTypes.Cashier).Wait();
                            break;
                        default:
                            break;
                    }

                    // add to table of businesscashiers and assign to business
                    DeliverNETUser userOwner = _userManager.FindByNameAsync((User.Identity.Name)).Result;
                    int businessId = _masterManager.GetBusinessOwnerManager().Get(userOwner.Id).BusinessId;
                    Business business = _masterManager.GetBusinessManager().Get(businessId);
                    _masterManager.GetBusinessCashierManager().Create(user, business);

                    return RedirectToAction("SettingsBusi");
                }
                ModelState.AddModelError(string.Empty, "New cashier registration failed.");
            }

            return View("SettingsBusi");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingsBusiVerify(SettingsBusiVerifyViewModel model)
        {
            if (ModelState.IsValid)
            {
                DeliverNETUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                BusinessOwner owner = _masterManager.GetBusinessOwnerManager().Get(user.Id);
                Business newBusiness = new Business()
                {
                    Title = model.Title,
                    Address = $"{model.Address}, {model.Area}, {model.City}",
                    PhoneNumber = model.PhoneNumber,
                    Email = user.Email,
                    VerificationDate = DateTime.Now,
                };

                bool result = _masterManager.GetBusinessManager().Create(newBusiness);

                if (result)
                    return RedirectToAction("SettingsBusi");
            }
            return View("SettingsBusi");
        }
    }
}