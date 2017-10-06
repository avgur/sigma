namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Data;

    using Microsoft.AspNet.Identity;

    public static class ApplicationUserExtender
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this FrontendUser user, UserManager<FrontendUser> manager)
        {
            // TODO[ds]: it looks like it never called
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }
}