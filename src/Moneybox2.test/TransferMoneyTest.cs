using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using System;

namespace Tests
{
    public class TransferMoneyTests
    {
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<INotificationService> notificationServiceMock;
        private Guid FromAccountGuid;
        private Guid ToAccountGuid;
        [SetUp]
        public void Setup()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationServiceMock = new Mock<INotificationService>();
            FromAccountGuid = new Guid("11111111-1111-1111-1111-111111111111");
            ToAccountGuid = new Guid("22222222-2222-2222-2222-222222222222");
        }


        [TestCase(1000, 1000, 0, 100, false, false)]
        [TestCase(3000, 1000, 1000, 3000, true, true)]
        [TestCase(4000, 1000, 1000, 3000, false, true)]
        [TestCase(3000, 1000, 0, 3000, true, false)]
        public void Transfer_Execute_Success(decimal startBalanceFrom, decimal startBalanceTo, decimal paidInTo, decimal amountToTransfer, bool hasNotificationFrom, bool hasNotificationTo)
        {
            var emailAddressFrom = "from@example.com";
            var emailAddressTo = "to@example.com";

            //Arrange
            accountRepositoryMock.Setup(x => x.GetAccountById(FromAccountGuid)).Returns(new Account
            (
                id: FromAccountGuid,
                balance: startBalanceFrom,
                user: new User(
                    email: emailAddressFrom
                )
            ));
            accountRepositoryMock.Setup(x => x.GetAccountById(ToAccountGuid)).Returns(new Account(
                id: ToAccountGuid,
                balance: startBalanceTo,
                paidIn: paidInTo,
                user: new User(
                    email: emailAddressTo
                )
            ));

            var transferMoney = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            //Act
            transferMoney.Execute(FromAccountGuid, ToAccountGuid, amountToTransfer);

            //Assert
            accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
            notificationServiceMock.Verify(x => x.NotifyFundsLow(emailAddressFrom), Times.Exactly(hasNotificationFrom ? 1 : 0));
            notificationServiceMock.Verify(x => x.NotifyApproachingPayInLimit(emailAddressTo), Times.Exactly(hasNotificationTo ? 1 : 0));
        }
        [TestCase(1000, 1000, 0, 1001, false, false)]
        [TestCase(0, 1000, 0, 1, false, false)]
        public void Transfer_Execute_NotSufficientFunds(decimal startBalanceFrom, decimal startBalanceTo, decimal paidInTo, decimal amountToTransfer, bool hasNotificationFrom, bool hasNotificationTo)
        {
            var emailAddressFrom = "from@example.com";
            var emailAddressTo = "to@example.com";

            //Arrange
            accountRepositoryMock.Setup(x => x.GetAccountById(FromAccountGuid)).Returns(new Account(
                id: FromAccountGuid,
                balance: startBalanceFrom,
                user: new User(
                    email: emailAddressFrom
                )
            ));
            accountRepositoryMock.Setup(x => x.GetAccountById(ToAccountGuid)).Returns(new Account(
                id: ToAccountGuid,
                balance: startBalanceTo,
                paidIn: paidInTo,
                user: new User(
                    email: emailAddressTo
                )
            ));

            var transferMoney = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            //Act
            void testDelegate() => transferMoney.Execute(FromAccountGuid, ToAccountGuid, amountToTransfer);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidOperationException>());
            accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Never());
            notificationServiceMock.Verify(x => x.NotifyFundsLow(emailAddressFrom), Times.Never());
            notificationServiceMock.Verify(x => x.NotifyApproachingPayInLimit(emailAddressTo), Times.Never());
        }
        [TestCase(3000, 1000, 3000, 1001, false, false)]
        [TestCase(1000, 1000, 4000, 1, false, false)]
        public void Transfer_Execute_PayInLimitExceeded(decimal startBalanceFrom, decimal startBalanceTo, decimal paidInTo, decimal amountToTransfer, bool hasNotificationFrom, bool hasNotificationTo)
        {
            var emailAddressFrom = "from@example.com";
            var emailAddressTo = "to@example.com";

            //Arrange
            accountRepositoryMock.Setup(x => x.GetAccountById(FromAccountGuid)).Returns(new Account(
                id: FromAccountGuid,
                balance: startBalanceFrom,
                user: new User(
                    email: emailAddressFrom
                )
            ));
            accountRepositoryMock.Setup(x => x.GetAccountById(ToAccountGuid)).Returns(new Account(
                id: ToAccountGuid,
                balance: startBalanceTo,
                paidIn: paidInTo,
                user: new User(
                    email: emailAddressTo
                )
            ));

            var transferMoney = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            //Act
            void testDelegate() => transferMoney.Execute(FromAccountGuid, ToAccountGuid, amountToTransfer);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidOperationException>());
            accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Never());
            notificationServiceMock.Verify(x => x.NotifyFundsLow(emailAddressFrom), Times.Never());
            notificationServiceMock.Verify(x => x.NotifyApproachingPayInLimit(emailAddressTo), Times.Never());
        }
    }
}