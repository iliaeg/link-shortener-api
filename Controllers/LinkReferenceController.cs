﻿using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

using LinkShortenerAPI.Repositories;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Helpers;

namespace LinkShortenerAPI.Controllers
{
    /// <summary>
    /// Link management controller.
    /// </summary>
    /// <remarks>Gets user information from the <see cref="HttpContext"/>.</remarks>
    [ApiController]
    [Authorize] // Require authenticated requests.
    [Route("api/v{version:apiVersion}/links")]
    [Route("api/links")]
    [ApiVersion("1.0")]
    public class LinkReferenceController : ControllerBase
    {
        private static readonly object LockerForCreation = new object();
        private static readonly object LockerForIncrement = new object();
        private readonly ILinkReferenceRepository linkReferenceRepository;
        private readonly IUserRepository userRepository;
        private readonly string baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceController"/> class.
        /// </summary>
        public LinkReferenceController(ILinkReferenceRepository linkReferenceRepository, IUserRepository userRepository, UrlSettings urlSettings)
        {
            this.linkReferenceRepository = linkReferenceRepository;
            this.userRepository = userRepository;
            baseUrl = urlSettings.BaseUrl;
        }

        /// <summary>
        /// Gets existing or creates a new link reference by original link in repository.
        /// </summary>
        [HttpGet("short")]
        public async Task<IActionResult> GetShortByOriginal([FromBody]ShortLinkRequest shortLinkRequest)
        {
            var user = await GetUserFromContextAsync(HttpContext);

            var linkRef = await linkReferenceRepository.GetByOriginal(shortLinkRequest.Url);

            if (linkRef != null)
            {
                return new JsonResult(linkRef.ShortLink);
            }

            lock (LockerForCreation)
            {
                // Set LinkShortener.Index if necessary.
                if (!LinkShortener.Index.HasValue)
                {
                    LinkShortener.Index = linkReferenceRepository.GetLastIndex();
                }

                linkRef = new LinkReference()
                {
                    OriginalLink = shortLinkRequest.Url,
                    ShortLink = string.Concat(baseUrl, '/', LinkShortener.CreateShortLink()),
                    LinkIndex = LinkShortener.Index.Value,
                    UserId = user.Id,
                };
            }

            await linkReferenceRepository.Create(linkRef);
            return new JsonResult(linkRef.ShortLink);
        }

        /// <summary>
        /// Gets original link by short and increase click counter.
        /// </summary>
        [HttpGet("original")]
        public async Task<IActionResult> GetOriginalByShort([FromBody] ShortLinkRequest shortLinkRequest)
        {
            var linkRef = await linkReferenceRepository.GetByShort(shortLinkRequest.Url);

            if (linkRef is null)
            {
                return new JsonErrorResult("Link reference for specified short link does not exist.", HttpStatusCode.BadRequest);
            }

            lock (LockerForIncrement)
            {
                linkReferenceRepository.IncreaseShortLinkCounter(linkRef.Id);
            }

            return new JsonResult(linkRef.OriginalLink);
        }

        /// <summary>
        /// Gets existing or creates a new link in repository.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLinks([FromQuery(Name = "limit")] int? limit = null, [FromQuery(Name = "start")] int? start = null)
        {
            var user = await GetUserFromContextAsync(HttpContext);

            var linkRefs = await linkReferenceRepository.GetLinks(user.Id, limit, start);

            var links =
                from linkRef in linkRefs
                select new {
                    OriginalLink = linkRef.OriginalLink,
                    ShortLink = linkRef.ShortLink,
                    Counter = linkRef.ShortLinkClickCounter,
                };

            return new JsonResult(links);
        }

        /// <summary>
        /// Returns short info about the api.
        /// </summary>
        [HttpGet("About")]
        public ContentResult About()
        {
            return Content("An API for getting short links.");
        }

        /// <summary>
        /// Gets user from the <see cref="HttpContext"/>.
        /// </summary>
        private async Task<User> GetUserFromContextAsync(HttpContext httpContext)
        {
            var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            var user = await userRepository.GetByEmail(email);
            return user;
        }
    }
}
