namespace FamilyAI.Domain.Models
{
    public enum PromptType
    {
        Educational,
        CSharp,
        Direct
    }
    public static class PromptTypeExtensions
    {
        public static string ToStringValue(this PromptType status)
        {
            return status switch
            {
                PromptType.Educational => "kids",
                PromptType.CSharp => "c#",
                PromptType.Direct => "lazy",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PromptType GetType(this string type)
        {
            return type switch
            {
                "kids" => PromptType.Educational,
                "c#" => PromptType.CSharp,
                "lazy" => PromptType.Direct,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }
}
