namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    using NHibernate.AspNet.Identity;

    public abstract class BaseUserManager<T> : UserManager<T>, IUserManager
        where T : IdentityUser
    {
        protected BaseUserManager(IUserStore<T> store)
            : base(store)
        {
        }

        async Task<IdentityUser> IUserManager.FindByNameAsync(string username)
        {
            var user = await this.FindByNameAsync(username);
            return user;
        }

        Task<bool> IUserManager.CheckPasswordAsync(IdentityUser user, string password)
        {
            return this.CheckPasswordAsync((T)user, password);
        }
    }
}