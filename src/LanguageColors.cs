using System.Windows.Media;

namespace GitHubLanguageStats;

internal static class LanguageColors
{
    public static Color Invalid { get; } = Colors.Black;

    public static Color C { get; } = HexToColor("555555");
    public static Color CPP { get; } = HexToColor("F34B7D");
    public static Color CS { get; } = HexToColor("178600");
    public static Color FS { get; } = HexToColor("B845FC");
    public static Color VisualBasic { get; } = HexToColor("945DB7");
    public static Color JavaScript { get; } = HexToColor("F1E05A");
    public static Color Python { get; } = HexToColor("3572A5");
    public static Color Lua { get; } = HexToColor("000080");
    public static Color Rust { get; } = HexToColor("DEA584");
    public static Color Ruby { get; } = HexToColor("701516");
    public static Color Squirrel { get; } = HexToColor("800000");
    public static Color Haxe { get; } = HexToColor("DF7900");
    public static Color Go { get; } = HexToColor("00ADD8");
    public static Color Vue { get; } = HexToColor("2C3E50");
    public static Color TypeScript { get; } = HexToColor("3178C6");
    public static Color Assembly { get; } = HexToColor("6E4C13");
    public static Color HTML { get; } = HexToColor("E34C26");
    public static Color CSS { get; } = HexToColor("663399");
    public static Color PHP { get; } = HexToColor("4F5D95");



    public static Color GetColorByName(string language)
    {
        switch (language.ToLower())
        {
            case "c": return C;
            case "c++": return CPP;
            case "c#": return CS;
            case "f#": return FS;
            case "visualbasic": return VisualBasic;
            case "javascript": return JavaScript;
            case "python": return Python;
            case "lua": return Lua;
            case "rust": return Rust;
            case "ruby": return Ruby;
            case "squirrel": return Squirrel;
            case "haxe": return Haxe;
            case "go": return Go;
            case "vue": return Vue;
            case "typescript": return TypeScript;
            case "assembly": return Assembly;
            case "html": return HTML;
            case "css": return CSS;
            case "php": return PHP;
            default: return Invalid;
        }
    }

    private static Color HexToColor(string hex) =>
        (Color)ColorConverter.ConvertFromString("#" + hex);
}
