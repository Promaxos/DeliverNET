using DeliverNET.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DeliverNET.Infrastructure.Account;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models.AccountViewModels;
using Microsoft.Extensions.Logging;
using DeliverNET.Managers;

namespace DeliverNET.Data
{
    public class Seeder
    {
        private readonly UserManager<DeliverNETUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<Seeder> _logger;
        private readonly DeliverNETClaimManager _claimManager;
        private readonly IMasterManager _masterManager;

        private IOrderManager _orderManager;
        private IDelivererManager _delivererManager;
        private IBusinessManager _businessManager;
        private IBusinessCashierManager _businessCashierManager;
        private IBusinessOwnerManager _businessOwnerManager;

        public Seeder(
            UserManager<DeliverNETUser> userManager,
            RoleManager<IdentityRole> roleManager,
            DeliverNETClaimManager claimManager,
            ILogger<Seeder> logger,
            IMasterManager masterManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _claimManager = claimManager;
            _masterManager = masterManager;
        }


        public async Task Seed()
        {
            // Initialize each manager
            _orderManager = _masterManager.GetOrderManager();
            _delivererManager = _masterManager.GetDelivererManager();
            _businessManager = _masterManager.GetBusinessManager();
            _businessCashierManager = _masterManager.GetBusinessCashierManager();
            _businessOwnerManager = _masterManager.GetBusinessOwnerManager();

            // Clear some data from db
            _orderManager.DeleteAll();



            //
            // Seed businesses
            //         
            List<Business> testBusinesses = new List<Business>()
            {
                new Business()
                {
                    Title = "Ραβαίσι",
                    Address = "Μιχαλακοπούλου 32, Ζωγράφου 15771",
                    Geolocation = "37.979567,23.757354",
                    Email = "rabaisi@hotmail.com",
                    SignupDate = DateTime.Today,
                    VerificationDate = DateTime.Today,
                    Active = true
                },
                new Business()
                {
                    Title = "BigBadWolf",
                    Address = "Λεωφ. Μεσογείων 202, Χολαργός 155 61",
                    Geolocation = "38.001952,23.790455",
                    Email = "bigbadwolf@hotmail.com",
                    SignupDate = DateTime.Today,
                    VerificationDate = DateTime.Today,
                    Active = true
                },
                new Business()
                {
                    Title = "Σάββας Κεμπάπ",
                    Address = "Ερμού 91, Αθήνα 105 55",
                    Geolocation = "37.976828,23.725088",
                    Email = "sabbaskebab@hotmail.com",
                    SignupDate = DateTime.Today,
                    VerificationDate = DateTime.Today,
                    Active = true
                }
            };
            Business tempBusiness;
            foreach (Business testBusiness in testBusinesses)
            {
                tempBusiness = _businessManager.Get(testBusiness.Title);
                if (tempBusiness == null)
                    _businessManager.Create(testBusiness);
            }


            //
            // Seed Roles
            //
            string[] Roles = { "admin", "user" };
            foreach (var role in Roles)
            {
                var res = await _roleManager.RoleExistsAsync(role);
                if (!res) { await _roleManager.CreateAsync(new IdentityRole(role)); }
            }

            //
            // Seed users
            //
            List<DeliverNETUser> testUsers = new List<DeliverNETUser>
            {
                new DeliverNETUser(){Email="admin@admins.com", UserName = "admin"},
                new DeliverNETUser(){Email="owner1@owners.com", UserName = "owner1"},
                new DeliverNETUser(){Email="owner2@owners.com", UserName = "owner2"},
                new DeliverNETUser(){Email="owner3@owners.com", UserName = "owner3"},
                new DeliverNETUser(){Email="cashier1@cashiers.com", UserName = "cashier1"},
                new DeliverNETUser(){Email="cashier2@cashiers.com", UserName = "cashier2"},
                new DeliverNETUser(){Email="slave1@slaves.com", UserName = "slave1"},
                new DeliverNETUser(){Email="slave2@slaves.com", UserName = "slave2"},
                new DeliverNETUser(){Email="slave3@slaves.com", UserName = "slave3"}
            };
            DeliverNETUser user;
            foreach (var tu in testUsers)
            {
                user = await _userManager.FindByEmailAsync(tu.Email);
                if (user != null)
                    continue;

                string password = tu.UserName;
                var result = await _userManager.CreateAsync(tu, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {tu.UserName} created succesfully.");

                    DeliverNETUser newUser = await _userManager.FindByEmailAsync(tu.Email);

                    if (password.Contains("admin"))
                        await _userManager.AddToRoleAsync(newUser, "admin");

                    await _userManager.AddToRoleAsync(newUser, "user");

                    if (password.Contains("owner"))
                    {
                        await _claimManager.AddClaim(newUser, JobTypes.Businessman);
                        switch (password)
                        {
                            case "owner1":
                                _businessOwnerManager.Create(newUser.Id, _businessManager.Get("Ραβαίσι").Id);
                                break;
                            case "owner2":
                                _businessOwnerManager.Create(newUser.Id, _businessManager.Get("BigBadWolf").Id);
                                break;
                            case "owner3":
                                _businessOwnerManager.Create(newUser.Id, _businessManager.Get("Σάββας Κεμπάπ").Id);
                                break;
                        }
                        // TODO Add Business owner records for each one
                    }
                    else if (password.Contains("slave"))
                    {
                        await _claimManager.AddClaim(newUser, JobTypes.Individual);
                        _delivererManager.Create(newUser);
                    }
                    else if (password.Contains("cashier"))
                    {
                        await _claimManager.AddClaim(newUser, JobTypes.Cashier);
                        int firstId = _businessManager.GetAll().First().Id;
                        int lastId = _businessManager.GetAll().Last().Id;
                        Business randomBusiness = _businessManager.Get(new Random().Next(firstId, lastId));
                        _businessCashierManager.Create(newUser, randomBusiness);
                    }
                }
                else
                {
                    _logger.LogInformation($"User {tu.UserName} was not created.");
                }
            }

            //
            // Seed Orders
            //           
            List<Order> testOrders = new List<Order>()
            {
                new Order()
                {
                    Business = _businessManager.Get("Ραβαίσι"),
                    Deliverer = _delivererManager.Get(
                       _userManager.FindByNameAsync("slave1").Result.Id
                        ),
                    Tstamp = DateTime.Now,
                    Address = "Αρχιλόχου 7",
                    Geolocation = "37.973217,23.773856",
                    FirstName = "Orestis",
                    LastName = "Koskoletos",
                    FloorNo = 1,
                    DoorName = "Koskoletas",
                    PhoneNumber = "6970456845",
                    PaymentTypeId = 0,
                    Price = 6.30f,
                    Comments = "Gamw to spitakis sou",
                    IsTimedOut = false
                },
                new Order(){
                Business = _businessManager.Get("BigBadWolf"),
                Deliverer = _delivererManager.Get(
                    _userManager.FindByNameAsync("slave3").Result.Id
                ),
                    Tstamp = DateTime.Now,
                    Address = "Λουλουδιών 33",
                    Geolocation = "37.973217,23.773856",
                    FirstName = "Μάριος",
                    LastName = "Ράδης",
                    FloorNo = 6,
                    DoorName = "Ραδής",
                    PhoneNumber = "6912345678",
                    PaymentTypeId = 0,
                    Price = 4.90f,
                    Comments = "μπεμπααααα",
                    IsTimedOut = false
                },
                new Order(){
                    Business = _businessManager.Get("Σάββας Κεμπάπ"),
                    Deliverer = _delivererManager.Get(
                        _userManager.FindByNameAsync("slave2").Result.Id
                        ),
                    Tstamp = DateTime.Now,
                    Address = "Μεσογείων 138",
                    Geolocation = "37.977217,23.763856",
                    FirstName = "Στάθης",
                    LastName = "Πανταζής",
                    FloorNo = 4,
                    DoorName = "Ραδής",
                    PhoneNumber = "6987654321",
                    PaymentTypeId = 1,
                    Price = 12.70f,
                    Comments = "μουνοπανοοο",
                    IsTimedOut = false
                }
            };
            foreach (Order testOrder in testOrders)
                _orderManager.Create(testOrder);
        }
    }
}

