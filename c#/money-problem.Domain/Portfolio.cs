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

    private static MissingExchangeRatesException ToException(IEnumerable<ConversionResult<MissingExchangeRateException>> results)
        => new(
            results
                .Where(result => result.HasException())
                .Select(result => result.GetExceptionUnsafe())
                .ToList());

    private static Money ToMoney(IEnumerable<ConversionResult<MissingExchangeRateException>> results, Currency currency)
        => new(results.Sum(result => result.GetMoneyUnsafe().Amount), currency);

    private static bool ContainsFailure(IEnumerable<ConversionResult<MissingExchangeRateException>> results)
        => results.Any(result => result.HasException());

    private List<ConversionResult<MissingExchangeRateException>> GetConvertedMoneys(Bank bank, Currency currency)
        => moneys
            .Select(money => ConvertMoney(bank, currency, money))
            .ToList();

    private static ConversionResult<MissingExchangeRateException> ConvertMoney(Bank bank, Currency currency, Money money)
    {
        try
        {
            return new ConversionResult<MissingExchangeRateException>(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException exception)
        {
            return new ConversionResult<MissingExchangeRateException>(exception);
        }
    }

    public class ConversionResult<T>
    {
        private readonly T? exception;

        private readonly Money? money;

        public ConversionResult(Money money)
            => this.money = money;

        public ConversionResult(T exception)
            => this.exception = exception;

        public bool HasException()
            => exception != null;

        public T GetExceptionUnsafe()
            => exception!;

        public bool HasMoney()
            => money != null;

        public Money GetMoneyUnsafe()
            => money!;
    }

    public ConversionResult<MissingExchangeRatesException> Evaluate(Bank bank, Currency currency)
    {
        try
        {
            var results = GetConvertedMoneys(bank, currency);
            return new ConversionResult<MissingExchangeRatesException>(ContainsFailure(results)
                ? throw ToException(results)
                : ToMoney(results, currency));
        }
        catch (MissingExchangeRatesException exception)
        {
            return new ConversionResult<MissingExchangeRatesException>(exception);
        }
    }
}