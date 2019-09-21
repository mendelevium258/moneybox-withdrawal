using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace Tests
{
    public class WithdrawMoneyTests
    {
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<INotificationService> notificationServiceMock;
        private Guid FromAccountGuid;
        [SetUp]
        public void Setup()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationServiceMock = new Mock<INotificationService>();
            FromAccountGuid = new Guid();
        }


        [TestCase(1000, 100, false)]
        [TestCase(1000, 1000, true)]
        [TestCase(1000, 500, false)]
        [TestCase(1000, 501, true)]
        [TestCase(3000, 3000, true)]
        [TestCase(4000, 3000, false)]
        [TestCase(500, 1, true)]
        public void Withdraw2_Execute_Success(decimal startBalance, decimal amountToWithdraw, bool hasNotification)
        {
            var emailAddress = "test@example.com";

            //Arrange
            accountRepositoryMock.Setup(x => x.GetAccountById(FromAccountGuid)).Returns(new Account(
                id: FromAccountGuid,
                user: new User(
                    email: emailAddress
                ),
                balance: startBalance
            ));

            var withdrawMoney = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            //Act
            withdrawMoney.Execute(FromAccountGuid, amountToWithdraw);

            //Assert
            accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Once());
            notificationServiceMock.Verify(x => x.NotifyFundsLow(emailAddress), Times.Exactly(hasNotification ? 1 : 0));
        }
        [TestCase(100, 101)]
        [TestCase(100, 1000)]
        public void Withdraw_Execute_NotSufficientFunds(decimal startBalance, decimal amountToWithdraw)
        {
            var emailAddress = "test0@example.com";

            //Arrange
            accountRepositoryMock.Setup(x => x.GetAccountById(FromAccountGuid)).Returns(new Account(
                id: FromAccountGuid,
                balance: startBalance,
                user: new User(
                    email: emailAddress
                )
            ));

            var withdrawMoney = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            //Act
            void testDelegate() => withdrawMoney.Execute(FromAccountGuid, amountToWithdraw);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidOperationException>());
            accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Never());
            notificationServiceMock.Verify(x => x.NotifyFundsLow(emailAddress), Times.Never());
        }
    }
}