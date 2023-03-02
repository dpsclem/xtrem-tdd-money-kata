using FluentAssertions;
using money_problem.Domain;
using Xunit;

namespace money_problem.Tests;

public class PortfolioShould
{
    private readonly Bank bank;

    public PortfolioShould()
    {
        bank = Bank.WithExchangeRate(Currency.EUR, Currency.USD, 1.2).AddExchangeRate(Currency.USD, Currency.KRW, 1100);
    }

    [Fact(DisplayName = "5 USD + 10 USD = 15 USD")]
    public void AddMoneyInSameCurrency()
    {
        var portfolio = new Portfolio();

        portfolio.Add(5.Dollars());
        portfolio.Add(10.Dollars());

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(15);
    }

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void AddMoneyInDollarAndEuro()
    {
        var portfolio = new Portfolio();
        portfolio.Add(5.Dollars());
        portfolio.Add(10.Euros());

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(17);
    }

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void AddMoneyInDollarAndKoreanWons()
    {
        var portfolio = new Portfolio();
        portfolio.Add(1.Dollars());
        portfolio.Add(1100.KoreanWons());

        var usdEvaluation = portfolio.Evaluate(Currency.KRW, bank);

        usdEvaluation.Should().Be(2200);
    }

    [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
    public void AddMoneyInDollarAndMultipleInEuros()
    {
        var portfolio = new Portfolio();
        portfolio.Add(5.Dollars());
        portfolio.Add(10.Euros());
        portfolio.Add(4.Euros());

        var usdEvaluation = portfolio.Evaluate(Currency.USD, bank);

        usdEvaluation.Should().Be(21.8);
    }

    [Fact(DisplayName = "Return missing exchange rates failure")]
    public void ReturnsMissingExchangeRatesFailure()
    {
        var portfolio = new Portfolio();
        portfolio.Add(1.Euros());
        portfolio.Add(1.Dollars());
        portfolio.Add(1.KoreanWons());

        var act = () => portfolio.Evaluate(Currency.EUR, bank);

        act.Should()
            .Throw<MissingExchangeRatesException>()
            .WithMessage("Missing exchange rate(s): [USD->EUR],[KRW->EUR]");
    }
}