using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App;
using Moneybox.App.Features;
using System;
using System.Collections.Generic;

namespace Moneybox.test
{
    [TestClass]
    public class TransferMoneyTest
    {
        [TestMethod]
        public void Transfer_Execute_Basic()
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
            Account toAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Balance = 0m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000001"),
                    Email = "test1@example.com",
                    Name = "test1"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount, toAccount });
            var notificationService = new MockNotificationService();

            var transferMoney = new TransferMoney(accountRepository, notificationService);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 100);

            Assert.AreEqual(900m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(100m, accountRepository.Accounts[1].Balance);
            Assert.AreEqual(-100m, accountRepository.Accounts[0].Withdrawn);
            Assert.AreEqual(100m, accountRepository.Accounts[1].PaidIn);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Expected Insufficient Funds")]
        public void Transfer_Execute_NotSufficientFunds()
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
            Account toAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Balance = 0m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000001"),
                    Email = "test1@example.com",
                    Name = "test1"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount, toAccount });
            var notificationService = new MockNotificationService();

            var transferMoney = new TransferMoney(accountRepository, notificationService);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 200);

            Assert.AreEqual(100m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(0m, accountRepository.Accounts[1].Balance);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Expected Pay In Limit Reached")]
        public void Transfer_Execute_PayInLimit()
        {
            Account fromAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Balance = 10000m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000000"),
                    Email = "test0@example.com",
                    Name = "test0"
                }
            };
            Account toAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Balance = 0m,
                PaidIn = 3000m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000001"),
                    Email = "test1@example.com",
                    Name = "test1"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount, toAccount });
            var notificationService = new MockNotificationService();

            var transferMoney = new TransferMoney(accountRepository, notificationService);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 1001);

            Assert.AreEqual(10000m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(0m, accountRepository.Accounts[1].Balance);
        }
        [TestMethod]
        public void Transfer_Execute_NotifyPayInLimit()
        {
            Account fromAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000000"),
                Balance = 10000m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000000"),
                    Email = "test0@example.com",
                    Name = "test0"
                }
            };
            Account toAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Balance = 0m,
                PaidIn = 3500m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000001"),
                    Email = "test1@example.com",
                    Name = "test1"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount, toAccount });
            var notificationService = new MockNotificationService();

            var transferMoney = new TransferMoney(accountRepository, notificationService);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 400);

            Assert.AreEqual(9600m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(400m, accountRepository.Accounts[1].Balance);

            Assert.AreEqual("test1@example.com", notificationService.LastNotificationAddress);
            Assert.AreEqual("Approaching Pay In Limit", notificationService.LastNotificationText);
        }
        [TestMethod]
        public void Transfer_Execute_NotifyFundsLow()
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
            Account toAccount = new Account
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Balance = 0m,
                PaidIn = 0m,
                Withdrawn = 0m,
                User = new User
                {
                    Id = new Guid("00000001-0000-0000-0000-000000000001"),
                    Email = "test1@example.com",
                    Name = "test1"
                }
            };
            var accountRepository = new MockAccountRepository(new List<Account> { fromAccount, toAccount });
            var notificationService = new MockNotificationService();

            var transferMoney = new TransferMoney(accountRepository, notificationService);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 400);

            Assert.AreEqual(100m, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(400m, accountRepository.Accounts[1].Balance);

            Assert.AreEqual("test0@example.com", notificationService.LastNotificationAddress);
            Assert.AreEqual("Funds Low", notificationService.LastNotificationText);
        }
    }
}
