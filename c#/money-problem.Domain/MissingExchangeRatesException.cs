namespace money_problem.Domain;

public class MissingExchangeRatesException : Exception
{
    public MissingExchangeRatesException(List<MissingExchangeRateException> missingExchangeRates) : base(
        $"Missing exchange rate(s): {GetExchangeRates(missingExchangeRates)}")
    {
    }

    private static string GetExchangeRates(List<MissingExchangeRateException> missingExchangeRates)
        => string.Join(",", missingExchangeRates.Select(x => $"[{x.Message}]"));
}