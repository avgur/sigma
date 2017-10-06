namespace Sigma.Web.Http.DataProtection
{
    using Microsoft.Owin.Security.DataProtection;

    public class MachineKeyDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new MachineKeyDataProtector(purposes);
        }
    }
}