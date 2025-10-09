public static class ExpandHelper
{
    public static bool ShouldExpand(List<string>? expands, string key)
        => expands?.Contains(key, StringComparer.OrdinalIgnoreCase) ?? false;
}
