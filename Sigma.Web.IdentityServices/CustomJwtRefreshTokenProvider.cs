namespace Sigma.Web.IdentityServices
{
    using System;
    using System.IdentityModel.Tokens;
    using System.Threading.Tasks;

    using Microsoft.Owin.Security.Infrastructure;

    public class CustomJwtRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            return Task.Factory.StartNew(() => this.Create(context));
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return Task.Factory.StartNew(() => this.Receive(context));
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            var audience = context.Ticket.Properties.Dictionary["audience"];

            if (string.IsNullOrEmpty(audience))
            {
                return;
            }

            // it will be set inside CustomJwtFromat accordingly to is's expiration configuration
            var expire = context.Request.Headers["expire"];
            context.Ticket.Properties.ExpiresUtc = expire != null
                ? (DateTimeOffset?)DateTimeOffset.UtcNow.AddSeconds(Convert.ToDouble(expire))
                : null;
            var token = context.SerializeTicket();
            context.SetToken(token);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            try
            {
                context.DeserializeTicket(context.Token);
            }
            catch (SecurityTokenException)
            {
                // smbdy tries to hack us
            }
            catch (SystemException)
            {
                // ArgumentException and all other staff in case totaly invalid token
            }
        }
    }
}
