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

            Assert.AreEqual(900, accountRepository.Accounts[0].Balance);
            Assert.AreEqual(100, accountRepository.Accounts[1].Balance);
            Assert.AreEqual(-100, accountRepository.Accounts[0].Withdrawn);
            Assert.AreEqual(100, accountRepository.Accounts[1].PaidIn);
        }
    }
}
