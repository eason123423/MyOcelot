using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyIdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyIdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public IActionResult Login(string Username, string Password)
        {
            var alice = _userManager.FindByNameAsync(Username).Result;
            if (alice != null)
            {
                var Claimsresult = _userManager.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                var result = _signInManager.PasswordSignInAsync(Username, Password, true, lockoutOnFailure: true).Result;
                if (result.Succeeded)
                {
                    var user = _userManager.FindByNameAsync(Username).Result;
                    _events.RaiseAsync(new UserLoginSuccessEvent(Username, user.Id, Username));
                    return Redirect("/WeatherForecast");
                }
                //var result =_signInManager.PasswordSignInAsync("i3yuan", "Li1591508445@#", true, lockoutOnFailure: true);
                //ApplicationUser user = new ApplicationUser();
                //user.UserName = "i3yuanaas";
                //user.PhoneNumber = "13522487713";
                //var ret = _userManager.CreateAsync(user, "Li1591508445@#").Result;
                return Ok(" ");
            }
            return Ok(" ");
        }
    }
}
