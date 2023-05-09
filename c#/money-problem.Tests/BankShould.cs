using FluentAssertions;
using money_problem.Domain;
using Xunit;
using static money_problem.Domain.Currency;
using FluentAssertions.LanguageExt;

namespace money_problem.Tests;

public class BankShould
{
    private readonly Bank _bank = Bank.WithExchangeRate(EUR, USD, 1.2);

    [Fact(DisplayName = "10 EUR -> USD = 12 USD")]
    public void ConvertEuroToUsd()
        => _bank.Convert(10.Euros(), USD)
            .Should()
            .Be(12.Dollars());

    [Fact(DisplayName = "10 EUR -> EUR = 10 EUR")]
    public void ConvertMoneyInSameCurrency()
        => _bank.Convert(10.Euros(), EUR)
            .Should()
            .Be(10.Euros());

    [Fact(DisplayName = "Throws a MissingExchangeRateException in case of missing exchange rates")]
    public void ReturnFailureWhenMissingExchangeRate()
        => _bank.Convert(10.Euros(), KRW)
            .Should()
            .Be("EUR->KRW");

    [Fact(DisplayName = "Conversion with different exchange rates EUR -> USD")]
    public void ConvertWithDifferentExchangeRates()
    {
        _bank.Convert(10.Euros(), USD)
            .Should()
            .Be(12.Dollars());

        _bank.AddExchangeRate(EUR, USD, 1.3)
            .Convert(10.Euros(), USD)
            .Should()
            .Be(13.Dollars());
    }
}