namespace Contracts;

public enum EmailTemplateType
{
    ConfirmEmail,
    ResetPassword
}

public record EmailRequested
{
    public string ToEmail { get; init; } = "";
    public string Subject { get; init; } = "";
    public EmailTemplateType Template { get; init; }
    public string? FirstName { get; init; }
    public string? ActionUrl { get; init; }
}
