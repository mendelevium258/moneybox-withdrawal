using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App;
using Moneybox.App.Features;
using System;
using System.Collections.Generic;

namespace Moneybox.test
{
    [TestClass]
    public class WithdrawMoneyTest
    {
        [TestMethod]
        public void Withdraw_Execute_Basic()
        {
            Account fromAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Balance = 1000m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000000"),
                    Email = "test0@example.com",
                    Name = "test0"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount });
            var notificationService = new MockNotificationService();

            var withdrawMoney = new WithdrawMoney(accountRepository, notificationService);
            withdrawMoney.Execute(fromAccount.Id, 100);

            Assert.AreEqual(900m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(-100m, accountRepository.Accounts[0].Withdrawn);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Expected Insufficient Funds")]
        public void Withdraw_Execute_NotSufficientFunds()
        {
            Account fromAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Balance = 100m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000000"),
                    Email = "test0@example.com",
                    Name = "test0"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount });
            var notificationService = new MockNotificationService();

            var withdrawMoney = new WithdrawMoney(accountRepository, notificationService);
            withdrawMoney.Execute(fromAccount.Id, 200);

            Assert.AreEqual(100m, accountRepository.Accounts[0].Balance);
        }
        [TestMethod]
        public void Withdraw_Execute_NotifyFundsLow()
        {
            Account fromAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Balance = 500m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000000"),
                    Email = "test0@example.com",
                    Name = "test0"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount });
            var notificationService = new MockNotificationService();

            var withdrawMoney = new WithdrawMoney(accountRepository, notificationService);
            withdrawMoney.Execute(fromAccount.Id, 400);

            Assert.AreEqual(100m, accountRepository.Accounts[0].Balance);

            Assert.AreEqual("test0@example.com", notificationService.LastNotificationAddress);
            Assert.AreEqual("Funds Low", notificationService.LastNotificationText);
        }
    }
}
