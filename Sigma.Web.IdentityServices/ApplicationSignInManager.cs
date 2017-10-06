namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Data;

    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    /*public class ApplicationSignInManager : SignInManager<FrontendUser, string>
    {
        public ApplicationSignInManager(FrontendUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(FrontendUser user)
        {
            return user.GenerateUserIdentityAsync((FrontendUserManager)this.UserManager);
        }
    }*/
}