using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortenerAPI.Helpers
{
    /// <summary>
    /// Аllows to return an error in the json format with a status code.
    /// </summary>
    public class JsonErrorResult : JsonResult
    {
        private readonly HttpStatusCode statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonErrorResult"/> class.
        /// </summary>
        /// <param name="json">Json object with error parameters.</param>
        /// <remarks>Status code is not specified, so it will be equals 500.</remarks>
        public JsonErrorResult(object json)
            : this(json, HttpStatusCode.InternalServerError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonErrorResult"/> class.
        /// </summary>
        /// <param name="json">Json object with error parameters.</param>
        /// <param name="statusCode">Http status code of the error.</param>
        public JsonErrorResult(object json, HttpStatusCode statusCode)
            : base(json)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonErrorResult"/> class.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="statusCode">Http status code of the error.</param>
        public JsonErrorResult(string errorMessage, HttpStatusCode statusCode)
            : base(new { error = errorMessage })
        {
            this.statusCode = statusCode;
        }

        /// <inheritdoc/>
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)statusCode;
            base.ExecuteResult(context);
        }

        /// <inheritdoc/>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)statusCode;
            return base.ExecuteResultAsync(context);
        }
    }
}
