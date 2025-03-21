# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WithdrawMoney.Execute(...).

## Guidelines

* You should spend no more than 1 hour on this task, although there is no time limit
* You should fork or copy this repository into your own public repository (Github, BitBucket etc.) before you do your work
* Your solution must compile and run first time
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed your work, send us a link to your public repository.

Good luck!

## Comments and Notes

There are a few parts that I am not entirely happy with and would like to improve given further time:
* Passing NotificationService by reference into Account.Withdraw means that Account needs to be aware of NotificationService
* The test methods create accounts each time which makes it harder to read what the code is actually doing
* The MockAccountRepository.GetAccountById should do a deep copy of the account (possibly using reflection) so when additional properties are added to Account, they are copied as well
* The MockAccountRepository.Update should be updated as above

In addition, I have noticed the following which would need verifying this against the spec/product owner.
* Amounts can be negative - which doesn't seem right
* Account.Withdrawn is negative where I was expecting it to be positive

## Comments and notes having looked at the other solutions

Having looked at the solutions that other people have posted, I have also made the following changes as I believe they enhance my solution
* Used Moq to mock all of the methods
* Switched the unit test to NUnit, which allows parameterised tests
* Changed the permissions of the Account and User classes to prevent updates
