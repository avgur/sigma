namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    using NHibernate.AspNet.Identity;

    public interface IUserManager
    {
        Task<IdentityUser> FindByNameAsync(string userName);

        Task<bool> IsLockedOutAsync(string userId);

        Task<bool> CheckPasswordAsync(IdentityUser user, string password);

        Task<IdentityResult> ResetAccessFailedCountAsync(string userId);

        Task<IdentityResult> AccessFailedAsync(string userId);
    }
}