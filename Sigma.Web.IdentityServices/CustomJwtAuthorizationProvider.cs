namespace Sigma.Web.IdentityServices
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;
    using System.Security.Principal;
    using System.IdentityModel.Tokens.Jwt;

    public class CustomJwtAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        private readonly string allowedAudience;

        public CustomJwtAuthorizationProvider(string allowedAudience)
        {
            this.allowedAudience = allowedAudience;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(this.allowedAudience);
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var username = context.UserName;
            var password = context.Password;

            /*
            var userManager = ServiceLocator.Current.GetInstance<IUserManager>(context.ClientId);
            var user = await userManager.FindByNameAsync(username);
            */
            var user = new ClaimsIdentity(new GenericIdentity(username), new[] { new Claim(ClaimTypes.Role, "divider") });

            var signInStatus = await CheckPasswordSignInAsync(/*userManager,*/ user, password, true/*Settings.Default.ShouldLockoutWhileAuth*/);
            GrantResourceOwnerCredentials(context, signInStatus, user, context.ClientId);
        }

        public override /*async*/ Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["audience"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult(0); 
            }

            var username = context.Ticket.Identity.Name;
            // var userManager = ServiceLocator.Current.GetInstance<IUserManager>(context.ClientId);
            // var user = string.IsNullOrEmpty(username) ? null : await userManager.FindByNameAsync(username);
            var user = new ClaimsIdentity(new GenericIdentity(username), new[] { new Claim(ClaimTypes.Role, "divider")});

            var signInStatus = user != null ? SignInStatus.Success : SignInStatus.Failure;
            GrantResourceOwnerCredentials(context, signInStatus, user, context.ClientId);

            return Task.FromResult(0);
        }

        
        private static /*async*/ Task<SignInStatus> CheckPasswordSignInAsync(
            /*IUserManager userManager,*/
            ClaimsIdentity user,
            string password,
            bool shouldLockout)
        {
            return Task.FromResult(SignInStatus.Success);
        /*
            SignInStatus signInStatus;
            if (user == null)
            {
                signInStatus = SignInStatus.Failure;
            }
            else if (await userManager.IsLockedOutAsync(user.Id))
            {
                signInStatus = SignInStatus.LockedOut;
            }
            else if (await userManager.CheckPasswordAsync(user, password))
            {
                // ReSharper disable once UnusedVariable
                var identityResult = await userManager.ResetAccessFailedCountAsync(user.Id);
                signInStatus = user.EmailConfirmed ? SignInStatus.Success : SignInStatus.RequiresVerification;
            }
            else
            {
                if (shouldLockout)
                {
                    // ReSharper disable once UnusedVariable
                    var identityResult = await userManager.AccessFailedAsync(user.Id);
                    if (await userManager.IsLockedOutAsync(user.Id))
                    {
                        signInStatus = SignInStatus.LockedOut;
                    }
                    else
                    {
                        signInStatus = SignInStatus.Failure;
                    }
                }
                else
                {
                    signInStatus = SignInStatus.Failure;
                }
            }

            return signInStatus;
          */
        }
        

        enum SignInStatus
        {
            Success,
            Failure,
            LockedOut,
            RequiresVerification
        }

        private static void GrantResourceOwnerCredentials(
            BaseValidatingTicketContext<OAuthAuthorizationServerOptions> context,
            SignInStatus status,
            ClaimsIdentity user,
            string clientId)
        {
            if (status == SignInStatus.Success)
            {
                var identity = new ClaimsIdentity("JWT");
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Name));
                // identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id));
                
                identity.AddClaims(
                    from claim in user.Claims
                    where
                        string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
                      //  ||
                      //  claim.ClaimType.Contains(SecurityExtensions.PermissionClaimTypePrefix)
                    select new Claim(claim.Type, claim.Value));
               
                var props =
                    new AuthenticationProperties(
                        new Dictionary<string, string> { { "audience", clientId ?? string.Empty } });

                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            }
            else
            {
                switch (status)
                {
                    case SignInStatus.Failure:
                        context.SetError("invalid_grant", "The user name or password is incorrect");
                        break;
                    case SignInStatus.LockedOut:
                        context.SetError("invalid_grant.locked", "The user account is locked");
                        break;
                    case SignInStatus.RequiresVerification:
                        context.SetError("invalid_grant.unverified", "The user account requires verification");
                        break;
                    default:
                        context.SetError("invalid_grant.error", "Unexpected failure. SignIn status: " + status);
                        break;
                }
            }
        }
    
    }
}
