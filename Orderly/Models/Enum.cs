namespace Orderly.Models
{
    public enum RoutineStatus
    {
        Ok,
        Warning,
        Error
    }

    public enum TerminalInputBehaviour
    {
        Insert,
        Copy
    }

    public enum CredentialBreachStatus
    {
        Unknown,
        Safe,
        Breached
    }

    public enum ExportFormats
    {
        TXT,
        PDF,
        CSV,
        HTML
    }
}
