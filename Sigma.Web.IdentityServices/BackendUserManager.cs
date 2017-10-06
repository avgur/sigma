namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Data;

    using Microsoft.AspNet.Identity;

    public class BackendUserManager : BaseUserManager<BackendUser>
    {
        public BackendUserManager(IUserStore<BackendUser> store)
            : base(store)
        {
        }

        public async Task<IdentityResult> CreateBackendUser(BackendUser user, string password)
        {
            var result = await this.CreateAsync(user, password);
            if (result.Succeeded)
            {
            }

            return result;
        }
    }
}
