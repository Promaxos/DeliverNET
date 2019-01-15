using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DeliverNET.Infrastructure.Account
{
    public class DeliverNETClaimManager
    {
        private readonly UserManager<DeliverNETUser> _userManager;
        private readonly ILogger<DeliverNETClaimManager> _logger;

        public DeliverNETClaimManager(
            UserManager<DeliverNETUser> userManager,
            ILogger<DeliverNETClaimManager> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task AddClaim(DeliverNETUser user, JobTypes jobType)
        {
            Claim c = new Claim("JobType",jobType.ToString());
            await _userManager.AddClaimAsync(user, c);
            _logger.LogInformation($"Added claims {c.Value} to user {user.UserName}");
        }

        public async Task<bool> HasClaim(DeliverNETUser user, JobTypes jobType)
        {
            bool hasClaim = false;
            // Get claims
            var claims = await _userManager.GetClaimsAsync(user);
            var requestedClaim = claims.Where(c => c.Value == jobType.ToString()).FirstOrDefault();
            if (requestedClaim != null)
                hasClaim = true;
            return hasClaim;
        }

        public async Task<bool> HasClaim(ClaimsPrincipal user, JobTypes jobType)
        {
            bool hasClaim = false;
            // Get claims
            var deliverNetUser = await _userManager.FindByNameAsync(user.Identity.Name);
            var claims = await _userManager.GetClaimsAsync(deliverNetUser);
            var requestedClaim = claims.Where(c => c.Value == jobType.ToString()).FirstOrDefault();
            if (requestedClaim != null)
                hasClaim = true;
            return hasClaim;
        }

    }
}
