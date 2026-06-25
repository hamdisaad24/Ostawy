namespace Ostawy.Helpers;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string Host { get; init; } = string.Empty;

    public int Port { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}