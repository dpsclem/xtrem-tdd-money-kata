namespace money_problem.Domain;

public class ConversionResult
{
    private readonly string? failureMessage;

    private readonly Money? money;

    public ConversionResult(Money money)
        => this.money = money;

    public ConversionResult(string failureMessage)
        => this.failureMessage = failureMessage;

    public bool HasFailure()
        => failureMessage != null;

    public string GetFailureUnsafe()
        => failureMessage!;

    public bool HasMoney()
        => money != null;

    public Money GetMoneyUnsafe()
        => money!;
}