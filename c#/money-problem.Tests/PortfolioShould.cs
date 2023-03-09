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
            .EvaluateWithException(bank, Currency.USD)
            .Should()
            .Be(15.Dollars());

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void AddMoneyInDollarAndEuro() =>
        PortfolioWith(5.Dollars(),10.Euros())
            .EvaluateWithException(bank, Currency.USD)
            .Should()
            .Be(17.Dollars());

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void AddMoneyInDollarAndKoreanWons()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(1.Dollars());
        portfolio = portfolio.Add(1100.KoreanWons());

        var usdEvaluation = portfolio.EvaluateWithException(bank, Currency.KRW);

        usdEvaluation.Should().Be(2200.KoreanWons());
    }

    [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
    public void AddMoneyInDollarAndMultipleInEuros()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(5.Dollars());
        portfolio = portfolio.Add(10.Euros());
        portfolio = portfolio.Add(4.Euros());

        var usdEvaluation = portfolio.EvaluateWithException(bank, Currency.USD);

        usdEvaluation.Should().Be(21.8.Dollars());
    }

    [Fact(DisplayName = "Return missing exchange rates failure")]
    public void ReturnsMissingExchangeRatesFailure()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(1.Euros());
        portfolio = portfolio.Add(1.Dollars());
        portfolio = portfolio.Add(1.KoreanWons());

        portfolio.Evaluate(bank, Currency.EUR).GetExceptionUnsafe().Should()
            .Be(new MissingExchangeRatesException("Missing exchange rate(s): [USD->EUR],[KRW->EUR]"));
    }
    
    private static Portfolio PortfolioWith(params Money[] moneys)
        => moneys.Aggregate(new Portfolio(), (current, money) => current.Add(money));
}