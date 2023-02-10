using System;
using System.Collections.Generic;
using System.Linq;
using money_problem.Domain;

namespace money_problem.Tests;

public class MissingExchangeRatesException : Exception
{
    public MissingExchangeRatesException(List<MissingExchangeRateException> missingExchangeRates) : base(
        $"Missing exchange rate(s): {GetExchangeRates(missingExchangeRates)}")
    {
    }

    private static string GetExchangeRates(List<MissingExchangeRateException> missingExchangeRates)
        => string.Join(",", missingExchangeRates.Select(x => $"[{x.Message}]"));
}