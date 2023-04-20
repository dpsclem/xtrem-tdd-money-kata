using System.Linq;
using FluentAssertions;
using money_problem.Domain;
using Xunit;

namespace money_problem.Tests;

public class PortfolioShould
{
    private readonly Bank bank;

    public PortfolioShould()
        => bank = Bank
               .WithExchangeRate(Currency.EUR, Currency.USD, 1.2)
               .AddExchangeRate(Currency.USD, Currency.KRW, 1100);

    [Fact(DisplayName = "5 USD + 10 USD = 15 USD")]
    public void AddMoneyInSameCurrency()
        => PortfolioWith(5.Dollars(), 10.Dollars())
            .Evaluate(bank, Currency.USD)
            .GetMoneyUnsafe()
            .Should()
            .Be(15.Dollars());

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void AddMoneyInDollarAndEuro()
        => PortfolioWith(5.Dollars(), 10.Euros())
            .Evaluate(bank, Currency.USD)
            .GetMoneyUnsafe()
            .Should()
            .Be(17.Dollars());

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void AddMoneyInDollarAndKoreanWons()
        => PortfolioWith(1.Dollars(), 1100.KoreanWons())
            .Evaluate(bank, Currency.KRW)
            .GetMoneyUnsafe()
            .Should()
            .Be(2200.KoreanWons());

    [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
    public void AddMoneyInDollarAndMultipleInEuros()
        => PortfolioWith(5.Dollars(), 10.Euros(), 4.Euros())
            .Evaluate(bank, Currency.USD)
            .GetMoneyUnsafe()
            .Should()
            .Be(21.8.Dollars());

    [Fact(DisplayName = "Return missing exchange rates failure")]
    public void ReturnsMissingExchangeRatesFailure()
        => PortfolioWith(1.Euros(), 1.Dollars(), 1.KoreanWons())
            .Evaluate(bank, Currency.EUR)
            .GetFailureUnsafe()
            .Should()
            .Be("Missing exchange rate(s): [USD->EUR],[KRW->EUR]");

    private static Portfolio PortfolioWith(params Money[] moneys)
        => moneys.Aggregate(new Portfolio(), (current, money) => current.Add(money));
}