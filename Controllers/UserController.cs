using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

using LinkShortenerAPI.Repositories;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Helpers;

namespace LinkShortenerAPI.Controllers
{
    /// <summary>
    /// User management controller.
    /// </summary>
    /// <remarks>For simplicity, uses Basic authorization with ObjectId as the authentication token.</remarks>
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
        /// Authenticates a user.
        /// </summary>
        [HttpPost("Auth")]
        public async Task<IActionResult> Authentication(AuthenticationRequest authenticationRequest)
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

            return new JsonResult(user.Id.ToString());

        }

        /// <summary>
        /// Returns short info about the api.
        /// </summary>
        [HttpGet("About")]
        public ContentResult About()
        {
            return Content("An API for registering and authenticating users in the service.");
        }

        // Admin/test tools

        /// <summary>
        /// Gets user by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return new JsonErrorResult("User id is invalid or not specified.", HttpStatusCode.BadRequest);
            }

            var user = await userRepository.Get(objectId);
            return new JsonResult(user);
        }

        /// <summary>
        /// Gets user by email.
        /// </summary>
        [HttpGet("ByEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            if (email is null)
            {
                return new JsonErrorResult("Email is null.", HttpStatusCode.BadRequest);
            }

            var user = await userRepository.GetByEmail(email);
            return new JsonResult(user);
        }
    }
}
