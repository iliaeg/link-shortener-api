using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson;

using LinkShortenerAPI.Repositories;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Helpers;

namespace LinkShortenerAPI.Controllers
{
    /// <summary>
    /// User management controller.
    /// </summary>
    /// <remarks>Uses cookies authorization.</remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [Route("api/users")]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Creates a new user in repository.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (await userRepository.GetByEmail(user.Email) != null)
            {
                return new JsonErrorResult("User with specified email already exists.", HttpStatusCode.BadRequest);
            }

            var id = await userRepository.Create(user);
            return new JsonResult(id.ToString());
        }

        /// <summary>
        /// Provides an option to log in. Authenticates a user.
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthenticationRequest authenticationRequest)
        {
            var user = await userRepository.GetByEmail(authenticationRequest.Email);
            if (user is null)
            {
                return new JsonErrorResult("Specified user not found.", HttpStatusCode.Unauthorized);
            }

            if (authenticationRequest.Password != user.Password)
            {
                return new JsonErrorResult("Password is incorrect.", HttpStatusCode.Unauthorized);
            }

            // Create claim for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, authenticationRequest.Email),
            };

            var identity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(identity);

            // Write auth cookies into context
            await HttpContext.SignInAsync("CookieAuth", principal);

            return NoContent();
        }

        /// <summary>
        /// Provides an option to log out.
        /// </summary>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return NoContent();
        }

        /// <summary>
        /// Returns short info about the api.
        /// </summary>
        [HttpGet("About")]
        public ContentResult About()
        {
            return Content("An API for registering and authenticating users in the service.");
        }
    }
}
