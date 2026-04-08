namespace FamilyAI.Domain.Models
{
    public enum FlagRuleType
    {
        Keyword = 0,
        Topic = 1,
        Tone = 2
    }

    public enum ScanSensitivity
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum FlagSource
    {
        Keyword = 0,
        AI = 1
    }
}
