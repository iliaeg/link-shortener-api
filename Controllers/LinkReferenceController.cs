using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

using LinkShortenerAPI.Repositories;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Helpers;

namespace LinkShortenerAPI.Controllers
{
    /// <summary>
    /// Link management controller.
    /// </summary>
    /// <remarks>For simplicity, uses Basic authorization with ObjectId as the authentication token.</remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/links")]
    [Route("api/links")]
    [ApiVersion("1.0")]
    public class LinkReferenceController : ControllerBase
    {
        private static readonly object LockerForCreation = new object();
        private static readonly object LockerForIncrement = new object();
        private readonly ILinkReferenceRepository linkReferenceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceController"/> class.
        /// </summary>
        public LinkReferenceController(ILinkReferenceRepository linkReferenceRepository)
        {
            this.linkReferenceRepository = linkReferenceRepository;
        }

        /// <summary>
        /// Gets existing or creates a new link reference by original link in repository.
        /// </summary>
        [HttpGet("short")]
        public async Task<IActionResult> GetShortByOriginal([FromBody]ShortLinkRequest shortLinkRequest)
        {
            var error = RequestValidationError(Request.Headers["Authorization"], out string userId, out ObjectId userObjectId);

            if (error != null)
            {
                return error;
            }

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
                    ShortLink = LinkShortener.CreateShortLink(),
                    LinkIndex = LinkShortener.Index.Value,
                    UserId = userObjectId,
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
            var error = RequestValidationError(Request.Headers["Authorization"], out string userId, out ObjectId userObjectId);

            if (error != null)
            {
                return error;
            }

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
            var error = RequestValidationError(Request.Headers["Authorization"], out string userId, out ObjectId userObjectId);

            if (error != null)
            {
                return error;
            }

            var linkRefs = await linkReferenceRepository.GetLinks(userObjectId, limit, start);

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
        /// Validates request and returns error in case of failure and null otherwise.
        /// </summary>
        private JsonErrorResult RequestValidationError(string authorizationHeader, out string userId, out ObjectId userObjectId)
        {
            userId = null;
            userObjectId = default;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
                userId = authHeader.Parameter;
            }
            catch (Exception)
            {
                return new JsonErrorResult("Authentication header value is invalid or not specified.", HttpStatusCode.Unauthorized);
            }

            // Here should be one more case if userId is valid, but user with such id does not exist.
            if (!ObjectId.TryParse(userId, out userObjectId))
            {
                return new JsonErrorResult("Authorization failed. User id is invalid or not specified.", HttpStatusCode.Unauthorized);
            }

            return null;
        }
    }
}
