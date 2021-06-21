using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Describes interface for Repository with the <see cref="LinksCounterRepository"/> object.
    /// </summary>
    public interface ILinksCounterRepository
    {
        /// <summary>
        /// Increments links counter.
        /// </summary>
        /// <remarks>Atomic operation in terms of mongodb.</remarks>
        LinksCounter IncrementCounter();
    }
}
