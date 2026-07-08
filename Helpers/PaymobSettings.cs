namespace Ostawy.Helpers;

public sealed class PaymobSettings
{
    public const string SectionName = "PAYMOB";

    public string APIKEY { get; init; } = string.Empty;

    public string HMAC { get; init; } = string.Empty;

    public string IFRAMEID { get; init; } = string.Empty;

    public string INTEGRATIONID { get; init; } = string.Empty;
}