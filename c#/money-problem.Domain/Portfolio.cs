namespace money_problem.Domain;

public class Portfolio
{
    private readonly ICollection<Money> moneys;

    public Portfolio()
        => moneys = new List<Money>();

    private Portfolio(List<Money> newMoneys)
        => moneys = newMoneys;

    public Portfolio Add(Money money)
    {
        var newMoneys = moneys.ToList();
        newMoneys.Add(money);
        return new Portfolio(newMoneys);
    }

    private static Money ToMoney(IEnumerable<ConversionResult> results, Currency currency)
        => new(results.Sum(result => result.Money.Amount), currency);

    private static bool ContainsFailure(IEnumerable<ConversionResult> results)
        => results.Any(result => result.HasFailure());

    private List<ConversionResult> GetConvertedMoneys(Bank bank, Currency currency)
        => moneys
            .Select(money => ConvertMoney(bank, currency, money))
            .ToList();

    private static ConversionResult ConvertMoney(Bank bank, Currency currency, Money money)
        => bank.Convert(money, currency);

    public ConversionResult Evaluate(Bank bank, Currency currency)
    {
        var results = GetConvertedMoneys(bank, currency);
        return ContainsFailure(results)
                   ? ToFailure(results)
                   : new ConversionResult(ToMoney(results, currency));
    }

    private static ConversionResult ToFailure(List<ConversionResult> results)
    {
        var missingExchangeRates = GetMissingExchangeRates(results);
        return new ConversionResult(
            $"Missing exchange rate(s): {string.Join(",", missingExchangeRates.Select(x => $"[{x}]"))}");
    }

    private static List<string> GetMissingExchangeRates(
        List<ConversionResult> results)
        => results
            .Where(result => result.HasFailure())
            .Select(result => result.Failure)
            .ToList();
}