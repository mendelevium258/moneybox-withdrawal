using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public Account(Guid id, User user, decimal balance = 0, decimal withdrawn = 0, decimal paidIn = 0)
        {
            Id = id;
            User = user;
            Balance = balance;
            Withdrawn = withdrawn;
            PaidIn = paidIn;
        }
        public void Withdraw(decimal amount, ref INotificationService notificationService)
        {
            var fromBalance = this.Balance - amount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (fromBalance < 500m)
            {
                notificationService.NotifyFundsLow(User.Email);
            }

            this.Balance -= amount;
            this.Withdrawn -= amount;
        }

        public void PayIn(decimal amount, ref INotificationService notificationService)
        {
            var paidIn = this.PaidIn + amount;
            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (Account.PayInLimit - paidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(User.Email);
            }

            this.Balance += amount;
            this.PaidIn += amount;
        }
    }
}
