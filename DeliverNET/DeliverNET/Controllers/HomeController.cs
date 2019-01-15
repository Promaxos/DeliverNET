using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DeliverNET.Models;
using DeliverNET.Models.HomeViewModels;
using DeliverNET.Services;
using Microsoft.AspNetCore.Authorization;

namespace DeliverNET.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AboutBusi()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AboutIndi()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                MailSender ms = new MailSender();
                await ms.SendEmailAsync(model.Email, "MessageFromContactUs", model.Message);
                // TODO It does not notify user for succesfull submit.
                return RedirectToAction("Index");
            }
            // TODO It does not return at the same area of the index view.
            return View("Index",model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
