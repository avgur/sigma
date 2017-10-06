namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Common.Localization;

    using Microsoft.AspNet.Identity;

    public class LocalizablePasswordValidator : PasswordValidator
    {
        private readonly IResourceProvider resourceProvider;

        public LocalizablePasswordValidator(IResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;
        }

        public override Task<IdentityResult> ValidateAsync(string item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(item) || item.Length < this.RequiredLength)
            {
                list.Add(string.Format(CultureInfo.CurrentCulture, this.resourceProvider.GetString("Identity_PasswordValidator_PasswordTooShort"), this.RequiredLength));
            }

            if (this.RequireNonLetterOrDigit
                && item.All(this.IsLetterOrDigit))
            {
                list.Add(this.resourceProvider.GetString("Identity_PasswordValidator_PasswordRequireNonLetterOrDigit"));
            }

            if (this.RequireDigit
                && item.All(c => !this.IsDigit(c)))
            {
                list.Add(this.resourceProvider.GetString("Identity_PasswordValidator_PasswordRequireDigit"));
            }

            if (this.RequireLowercase
                && item.All(c => !this.IsLower(c)))
            {
                list.Add(this.resourceProvider.GetString("Identity_PasswordValidator_PasswordRequireLower"));
            }

            if (this.RequireUppercase
                && item.All(c => !this.IsUpper(c)))
            {
                list.Add(this.resourceProvider.GetString("Identity_PasswordValidator_PasswordRequireUpper"));
            }

            if (list.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(string.Join(" ", list)));
        }
    }
}
