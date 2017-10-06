namespace Sigma.Web.IdentityServices
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Microsoft.Owin.Security.Jwt;

    using Thinktecture.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;

    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "audience";

        private readonly string knownAudience;

        private readonly string issuer;

        private readonly string base64Secret;

        private readonly int accessTokenExpireSeconds;

        private readonly Func<Claim, bool> claimFilter;

        static CustomJwtFormat()
        {
            //JwtSecurityTokenHandler.InboundClaimTypeMap.Add("sid", SecurityExtensions.DefaultUserIdClaimType);
            /*JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/actAs/client", SecurityExtensions.ToActAsPermission(OrganizationActAs.Client.ToString("G")));
            JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/actAs/customer", SecurityExtensions.ToActAsPermission(OrganizationActAs.Customer.ToString("G")));
            JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/owner", SecurityExtensions.ToPermissionClaimName("owner"));
            JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/special", SecurityExtensions.ToPermissionClaimName("special"));
            JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/test", SecurityExtensions.ToPermissionClaimName("test"));
            JwtSecurityTokenHandler.InboundClaimTypeMap.Add("permissions/production", SecurityExtensions.ToPermissionClaimName("production"));*/

            //JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.DefaultUserIdClaimType, "sid");
            /*JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToActAsPermission(OrganizationActAs.Client.ToString("G")), "permissions/actAs/client");
            JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToActAsPermission(OrganizationActAs.Customer.ToString("G")), "permissions/actAs/customer");
            JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToPermissionClaimName("owner"), "permissions/owner");
            JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToPermissionClaimName("special"), "permissions/special");
            JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToPermissionClaimName("test"), "permissions/test");
            JwtSecurityTokenHandler.OutboundClaimTypeMap.Add(SecurityExtensions.ToPermissionClaimName("production"), "permissions/production");*/
        }

        public CustomJwtFormat(string knownAudience, string issuer, string base64Secret, int accessTokenExpireSeconds, Func<Claim, bool> claimFilter = null)
        {
            this.knownAudience = knownAudience;
            this.issuer = issuer;
            this.base64Secret = base64Secret;
            this.accessTokenExpireSeconds = accessTokenExpireSeconds;
            this.claimFilter = claimFilter ?? (c => true);
        }

        public string Protect(AuthenticationTicket data)
        {
            Contract.Assert(data != null);

            var audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) 
                ? data.Properties.Dictionary[AudiencePropertyKey] : null;

            if (string.IsNullOrWhiteSpace(audienceId))
            {
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");
            }

            var keyByteArray = TextEncodings.Base64Url.Decode(this.base64Secret);
            var securityKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature);

            var issued = data.Properties.IssuedUtc ?? DateTimeOffset.UtcNow;
            var expires = data.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddSeconds(this.accessTokenExpireSeconds);

            var token = new JwtSecurityToken(
                this.issuer, 
                audienceId, 
                data.Identity.Claims.Where(this.claimFilter), 
                issued.UtcDateTime, 
                expires.UtcDateTime,
                signingCredentials);
            
            var handler = new JwtSecurityTokenHandler();
            
            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(protectedText));

            var keyByteArray = TextEncodings.Base64Url.Decode(this.base64Secret);
            SecurityKey signingKey = new SymmetricSecurityKey(keyByteArray);
            
            var validationParameters = new TokenValidationParameters
            {
                AuthenticationType = "JWT",
                ValidAudiences = new[] { this.knownAudience },
                ValidIssuers = new[] { this.issuer },

                IssuerSigningKey = signingKey,
                NameClaimType = ClaimTypes.NameIdentifier //JwtRegisteredClaimNames.Sub
            };

            SecurityToken validatedToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = (ClaimsIdentity)tokenHandler.ValidateToken(protectedText, validationParameters, out validatedToken).Identity;
            
            var properties = new AuthenticationProperties { AllowRefresh = false };
            if (validatedToken.ValidFrom != DateTime.MinValue)
            {
                properties.IssuedUtc = validatedToken.ValidFrom.ToUniversalTime();
            }

            if (validatedToken.ValidTo != DateTime.MinValue)
            {
                properties.ExpiresUtc = validatedToken.ValidTo.ToUniversalTime();
            }

            var token = (JwtSecurityToken)validatedToken;
            // properties.Dictionary.Add("sub", token.Subject);
            properties.Dictionary.Add("audience", token.Audiences.FirstOrDefault());

            return new AuthenticationTicket(identity, properties);
        }
    }
}
