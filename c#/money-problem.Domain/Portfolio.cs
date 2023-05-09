using LanguageExt;

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

    private ConversionResult EvaluateWithConversionResult(Bank bank, Currency currency)
    {
        var results = GetConvertedMoneys(bank, currency);
        return ContainsFailure(results)
                   ? ConversionResult.FromFailure(ToFailure(results))
                   : ConversionResult.FromMoney(ToSuccess(results, currency));
    }

    private static Money ToSuccess(IEnumerable<ConversionResult> results, Currency currency)
        => new(results.Where(result => result.HasSuccess()).Sum(result => result.Money!.Amount), currency);

    private static string ToFailure(IEnumerable<ConversionResult> results)
        => $"Missing exchange rate(s): {GetMissingRates(results.Where(result => result.HasFailure()).Select(result => result.Failure!))}";

    private static string GetMissingRates(IEnumerable<string> missingRates)
        => missingRates
            .Select(value => $"[{value}]")
            .Aggregate((r1, r2) => $"{r1},{r2}");

    public Either<string, Money> Evaluate(Bank bank, Currency currency)
    {
        var result = EvaluateWithConversionResult(bank, currency);
        return result.HasFailure()
                   ? result.Failure!
                   : result.Money!;
    }
}