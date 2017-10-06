namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Infoplus.TenderMarket.Common;
    using Infoplus.TenderMarket.Common.Localization;

    using Microsoft.AspNet.Identity;

    using NHibernate.AspNet.Identity;

    public class LocalizableUserValidator<TUser> : UserValidator<TUser, string>
        where TUser : class, IUser<string> 
    {
        private readonly UserManager<TUser, string> manager;

        private readonly IResourceProvider resourceProvider;

        public LocalizableUserValidator(UserManager<TUser, string> manager, IResourceProvider resourceProvider)
            : base(manager)
        {
            this.manager = manager;
            this.resourceProvider = resourceProvider;
        }

        public async override Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var identityUser = item as IdentityUser;
            if (identityUser == null)
            {
                return await base.ValidateAsync(item);
            }
            else
            {
                var errors = new List<string>();
                await this.ValidateUserName(identityUser, errors).WithCurrentCulture();
                if (this.RequireUniqueEmail)
                {
                    await this.ValidateEmailAsync(identityUser, errors).WithCurrentCulture();
                }

                return errors.Count <= 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
            }
        }

        private async Task ValidateUserName(IdentityUser user, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.PropertyTooShort", "Name"));*/
                errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Username_TooShort"));
            }
            else if (this.AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, "^[A-Za-z0-9@_\\.]+$"))
            {
                /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidUserName", user.UserName));*/
                errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Username_AllowOnlyAlphanumeric").Fmt(user.UserName));
            }
            else
            {
                var owner = await this.manager.FindByNameAsync(user.UserName).WithCurrentCulture();
                if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
                {
                    /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.DuplicateName", user.UserName));*/
                    errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Username_Duplicated").Fmt(user.UserName));
                }
            }
        }

        private async Task ValidateEmailAsync(IdentityUser user, ICollection<string> errors)
        {
            var email = user.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.PropertyTooShort", "Email"));*/
                errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Email_TooShort"));
            }
            else
            {
                try
                {
                    // ReSharper disable once UnusedVariable
                    var mailAddress = new MailAddress(email);
                }
                catch (FormatException)
                {
                    /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidEmail", email));*/
                    errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Email_Invalid").Fmt(email));

                    // do not want to check email duplication in case of invalid email
                    return;
                }

                var owner = await this.manager.FindByEmailAsync(email).WithCurrentCulture();
                if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
                {
                    /*errors.Add(string.Format(CultureInfo.CurrentCulture, "Resources.DuplicateEmail", email));*/
                    errors.Add(this.resourceProvider.GetString("Identity_UserValidator_Email_Duplicated").Fmt(email));
                }
            }
        }
    }
}
