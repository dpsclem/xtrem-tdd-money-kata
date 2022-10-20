﻿using money_problem.Domain;
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
    }
}