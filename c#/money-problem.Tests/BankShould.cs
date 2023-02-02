using System;
using FluentAssertions;
using money_problem.Domain;
using Xunit;
using static money_problem.Domain.Currency;

namespace money_problem.Tests
{
    public class BankShould
    {
        private readonly Bank _bank = Bank.WithExchangeRate(EUR, USD, 1.2);

        [Fact(DisplayName = "10 EUR -> USD = 12 USD")]
        public void ConvertEuroToUsd()
        {
            var money = new Money(10, EUR);
            _bank.Convert(money, USD)
                .Should()
                .Be(12);
        }

        [Fact(DisplayName = "10 EUR -> EUR = 10 EUR")]
        public void ConvertMoneyInSameCurrency()
        {
            var money = new Money(10, EUR);
            _bank.Convert(money, EUR)
                .Should()
                .Be(10);
        }

        [Fact(DisplayName = "Throws a MissingExchangeRateException in case of missing exchange rates")]
        public void ConvertWithMissingExchangeRateShouldThrowException()
        {
            var money = new Money(10, EUR);
            _bank.Invoking(_=>_.Convert(money, KRW))
                .Should()
                .ThrowExactly<MissingExchangeRateException>()
                .WithMessage("EUR->KRW");
        }

        [Fact(DisplayName = "Conversion with different exchange rates EUR -> USD")]
        public void ConvertWithDifferentExchangeRates()
        {
            var money = new Money(10, EUR);
            _bank.Convert(money, USD)
                .Should()
                .Be(12);

            _bank.AddExchangeRate(EUR, USD, 1.3);
            
            _bank.Convert(money, USD)
                .Should()
                .Be(13);
        }
    }
}