using System;
using System.Collections.Generic;
using System.Linq;
using Moneybox.App;
using Moneybox.App.DataAccess;

namespace Moneybox.test
{
    internal class MockAccountRepository : IAccountRepository
    {
        public List<Account> Accounts { get; set; }

        public MockAccountRepository(List<Account> accounts)
        {
            this.Accounts = accounts;
        }

        public Account GetAccountById(Guid accountId)
        {
            var account = Accounts.First(x => x.Id == accountId);
            //We need to return a copy of the account object
            var account2 = new Account
            {
                Balance = account.Balance,
                Id = account.Id,
                PaidIn = account.PaidIn,
                Withdrawn = account.Withdrawn,
                User = account.User
            };
            return account2;
        }

        public void Update(Account account)
        {
            var accountToUpdate = Accounts.First(x => x.Id == account.Id);
            //This should be improved to copy all properties, not just the ones listed
            accountToUpdate.Balance = account.Balance;
            accountToUpdate.PaidIn = account.PaidIn;
            accountToUpdate.Withdrawn = account.Withdrawn;
        }
    }
}