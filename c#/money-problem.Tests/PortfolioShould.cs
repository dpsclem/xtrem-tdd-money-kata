using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using money_problem.Domain;
using Xunit;

namespace money_problem.Tests;

public class PortfolioShould
{
    private readonly Bank bank;

    public PortfolioShould()
    {
        bank = Bank.WithExchangeRate(Currency.EUR, Currency.USD, 1.2);
        bank.AddExchangeRate(Currency.USD, Currency.KRW, 1100);
    }

    [Fact(DisplayName = "5 USD + 10 USD = 15 USD")]
    public void AddMoneyInSameCurrency()
    {
        var portfolio = new Portfolio();

        portfolio.Add(new Money(5, Currency.USD));
        portfolio.Add(new Money(10, Currency.USD));

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(15);
    }

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void AddMoneyInDollarAndEuro()
    {
        var portfolio = new Portfolio();
        portfolio.Add(new Money(5, Currency.USD));
        portfolio.Add(new Money(10, Currency.EUR));

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(17);
    }

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void AddMoneyInDollarAndKoreanWons()
    {
        var portfolio = new Portfolio();
        portfolio.Add(new Money(1, Currency.USD));
        portfolio.Add(new Money(1100, Currency.KRW));

        var usdEvaluation = portfolio.Evaluate(Currency.KRW, bank);

        usdEvaluation.Should().Be(2200);
    }

    [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
    public void AddMoneyInDollarAndMultipleInEuros()
    {
        var portfolio = new Portfolio();
        portfolio.Add(new Money(5, Currency.USD));
        portfolio.Add(new Money(10, Currency.EUR));
        portfolio.Add(new Money(4, Currency.EUR));

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(21.8);
    }

    [Fact(DisplayName = "Return missing exchange rates failure")]
    public void ReturnsMissingExchangeRatesFailure()
    {
        var portfolio = new Portfolio();
        portfolio.Add(new Money(1, Currency.EUR));
        portfolio.Add(new Money(1, Currency.USD));
        portfolio.Add(new Money(1, Currency.KRW));

        var act = () => portfolio.Evaluate(Currency.EUR, bank);

        act.Should()
            .Throw<MissingExchangeRatesException>()
            .WithMessage("Missing exchange rate(s): [USD->EUR],[KRW->EUR]");
    }
}

public class MissingExchangeRatesException : Exception
{
    public MissingExchangeRatesException(List<MissingExchangeRateException> missingExchangeRates) : base(
        $"Missing exchange rate(s): {GetExchangeRates(missingExchangeRates)}")
    {
    }

    private static string GetExchangeRates(List<MissingExchangeRateException> missingExchangeRates)
        => string.Join(",", missingExchangeRates.Select(x => $"[{x.Message}]"));
}

public class Portfolio
{
    private readonly List<Money> moneys = new();

    public void Add(Money money)
        => moneys.Add(money);

    public double Evaluate(Currency currency, Bank bank)
    {
        double total = 0;
        var missingExchangeRates = new List<MissingExchangeRateException>();
        foreach (var money in moneys)
            try
            {
                total += bank.Convert(money, currency);
            }
            catch (MissingExchangeRateException missingExchangeRate)
            {
                missingExchangeRates.Add(missingExchangeRate);
            }

        if (missingExchangeRates.Any())
            throw new MissingExchangeRatesException(missingExchangeRates);

        return total;
    }
}