namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Business;
    using Infoplus.TenderMarket.Data;

    using Microsoft.AspNet.Identity;

    public class FrontendUserManager : BaseUserManager<FrontendUser>
    {
        private readonly IRegistrationManager registrationManger;

        public FrontendUserManager(IUserStore<FrontendUser> store, IRegistrationManager registrationManger)
            : base(store)
        {
            this.registrationManger = registrationManger;
        }

        public async Task<IdentityResult> CreateFrontendUser(FrontendUser user, string password, IFullOrganizationRegistrationData data)
        {
            var result = await this.CreateAsync(user, password);
            if (result.Succeeded)
            {
                this.registrationManger.Register(user, data, true);
            }

            return result;
        }
    }
}
