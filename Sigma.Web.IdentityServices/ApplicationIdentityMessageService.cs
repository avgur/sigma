namespace Infoplus.TenderMarket.Web.IdentityServices
{
    using System.Threading.Tasks;

    using global::Common.Logging;

    using Infoplus.TenderMarket.Business;

    using Microsoft.AspNet.Identity;

    public class ApplicationIdentityMessageService : IIdentityMessageService
    {
        public INotificationManager NotificationManager { get; set; }
        
        public Task SendAsync(IdentityMessage message)
        {
            this.NotificationManager.SendEmail(message.Destination, message.Subject, message.Body);
            
            return Task.FromResult(0);
        }
    }
}
