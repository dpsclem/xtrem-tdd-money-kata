using money_problem.Tests;

namespace money_problem.Domain;

public class Portfolio
{
    private readonly ICollection<Money> moneys;

    public Portfolio()
    {
        moneys = new List<Money>();
    }

    private Portfolio(List<Money> newMoneys)
    {
        moneys = newMoneys;
    }

    public Portfolio Add(Money money)
    {
        List<Money> newMoneys = moneys.ToList();
        newMoneys.Add(money);
        return new Portfolio(newMoneys);
    }

    public Money Evaluate(Currency currency, Bank bank)
    {
        var results = GetConvertedMoneys(bank, currency);
        return ContainsFailure(results)
                   ? throw ToException(results)
                   : ToMoney(results, currency);
    }

    private static MissingExchangeRatesException ToException(IEnumerable<ConversionResult> results)
        => new(
            results
                .Where(result => result.HasException())
                .Select(result => result.GetExceptionUnsafe())
                .ToList());

    private static Money ToMoney(IEnumerable<ConversionResult> results, Currency currency)
        => new(results.Sum(result => result.GetMoneyUnsafe().Amount), currency);

    private static bool ContainsFailure(IEnumerable<ConversionResult> results)
        => results.Any(result => result.HasException());

    private List<ConversionResult> GetConvertedMoneys(Bank bank, Currency currency)
        => moneys
            .Select(money => ConvertMoney(bank, currency, money))
            .ToList();

    private static ConversionResult ConvertMoney(Bank bank, Currency currency, Money money)
    {
        try
        {
            return new ConversionResult(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException exception)
        {
            return new ConversionResult(exception);
        }
    }

    private class ConversionResult
    {
        private readonly MissingExchangeRateException? exception;

        private readonly Money? money;

        public ConversionResult(Money money)
            => this.money = money;

        public ConversionResult(MissingExchangeRateException exception)
            => this.exception = exception;

        public bool HasException()
            => exception != null;

        public MissingExchangeRateException GetExceptionUnsafe()
            => exception!;

        public bool HasMoney()
            => money != null;

        public Money GetMoneyUnsafe()
            => money!;
    }
}