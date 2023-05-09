namespace money_problem.Domain;

internal class ConversionResult
{
    public ConversionResult(Money money)
        => Money = money;

    public ConversionResult(string failure)
        => Failure = failure;

    public string? Failure { get; }

    public Money? Money { get; }

    public bool HasFailure()
        => Failure is not null;

    public bool HasSuccess()
        => Money is not null;

    public static ConversionResult FromFailure(string failure)
        => new(failure);

    public static ConversionResult FromMoney(Money money)
        => new(money);
}