namespace Sigma.Web.Http.DataProtection
{
    using System.Web.Security;

    using Microsoft.Owin.Security.DataProtection;

    /// <summary>
    /// Protector aim to work inside webfarm
    /// Don't forget to sync machine keys
    /// </summary>
    public class MachineKeyDataProtector : IDataProtector
    {
        private readonly string[] purposes;

        public MachineKeyDataProtector(string[] purposes)
        {
            this.purposes = purposes;
        }

        public byte[] Protect(byte[] userData)
        {
            return MachineKey.Protect(userData, this.purposes);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return MachineKey.Unprotect(protectedData, this.purposes);
        }
    }
}