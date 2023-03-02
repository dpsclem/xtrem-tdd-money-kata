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
        bank = bank.AddExchangeRate(Currency.USD, Currency.KRW, 1100);
    }

    [Fact(DisplayName = "5 USD + 10 USD = 15 USD")]
    public void AddMoneyInSameCurrency()
    {
        var portfolio = new Portfolio();

        portfolio = portfolio.Add(5.Dollars());
        portfolio = portfolio.Add(10.Dollars());

        var usdEvaluation = portfolio.Evaluate(bank, Currency.USD);

        usdEvaluation.Should().Be(15.Dollars());
    }

    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void AddMoneyInDollarAndEuro()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(5.Dollars());
        portfolio = portfolio.Add(10.Euros());

        var usdEvaluation = portfolio.Evaluate(bank, Currency.USD);

        usdEvaluation.Should().Be(17.Dollars());
    }

    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void AddMoneyInDollarAndKoreanWons()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(1.Dollars());
        portfolio = portfolio.Add(1100.KoreanWons());

        var usdEvaluation = portfolio.Evaluate(bank, Currency.KRW);

        usdEvaluation.Should().Be(2200.KoreanWons());
    }

    [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
    public void AddMoneyInDollarAndMultipleInEuros()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(5.Dollars());
        portfolio = portfolio.Add(10.Euros());
        portfolio = portfolio.Add(4.Euros());

        var usdEvaluation = portfolio.Evaluate(bank, Currency.USD);

        usdEvaluation.Should().Be(21.8.Dollars());
    }

    [Fact(DisplayName = "Return missing exchange rates failure")]
    public void ReturnsMissingExchangeRatesFailure()
    {
        var portfolio = new Portfolio();
        portfolio = portfolio.Add(1.Euros());
        portfolio = portfolio.Add(1.Dollars());
        portfolio = portfolio.Add(1.KoreanWons());

        var act = () => portfolio.Evaluate(bank, Currency.EUR);

        act.Should()
            .Throw<MissingExchangeRatesException>()
            .WithMessage("Missing exchange rate(s): [USD->EUR],[KRW->EUR]");
    }
}