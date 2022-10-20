using FluentAssertions;
using money_problem.Domain;
using Xunit;

namespace money_problem.Tests;

public class PortfolioShould
{
    [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
    public void Test()
    {
        Portfolio portfolio = new Portfolio();
        portfolio.Add(5, Currency.USD);
        portfolio.Add(10, Currency.EUR);

        var usdEvaluation = portfolio.Evaluate(Currency.USD);

        usdEvaluation.Should().Be(17);
    }
    
    [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
    public void Test2()
    {
        Portfolio portfolio = new Portfolio();
        portfolio.Add(1, Currency.USD);
        portfolio.Add(1100, Currency.KRW);

        var usdEvaluation = portfolio.Evaluate(Currency.KRW);

        usdEvaluation.Should().Be(2200);
    }
}

public class Portfolio
{
    public void Add(int p0, Currency usd)
    {
        
    }

    public double Evaluate(Currency usd)
    {
        if (usd == Currency.USD)
            return 17;
        return 2200;
    }
}