using Moneybox.App.Domain.Services;

namespace Moneybox.test
{
    internal class MockNotificationService : INotificationService
    {
        public string LastNotificationAddress { get; set; }
        public string LastNotificationText { get; set; }
        public MockNotificationService()
        {
            LastNotificationAddress = "";
            LastNotificationText = "";
        }

        public void NotifyApproachingPayInLimit(string emailAddress)
        {
            LastNotificationAddress = emailAddress;
            LastNotificationText = "Approaching Pay In Limit";
        }

        public void NotifyFundsLow(string emailAddress)
        {
            LastNotificationAddress = emailAddress;
            LastNotificationText = "Funds Low";
        }
    }
}