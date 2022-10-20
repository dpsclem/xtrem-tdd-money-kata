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

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void Test()
    {
        var portfolio = new Portfolio();
        portfolio.Add(5, Currency.USD);
        portfolio.Add(10, Currency.EUR);

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(17);
    }

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void Test2()
    {
        var portfolio = new Portfolio();
        portfolio.Add(1, Currency.USD);
        portfolio.Add(1100, Currency.KRW);

        var usdEvaluation = portfolio.Evaluate(Currency.KRW, bank);

        usdEvaluation.Should().Be(2200);
    }
}

public class Portfolio
{
    private readonly List<(int, Currency)> moneys = new();

    public void Add(int amount, Currency currency)
        => moneys.Add((amount, currency));

    public double Evaluate(Currency currency, Bank bank)
        => moneys.Sum(money => bank.Convert(money.Item1, money.Item2, currency));
}