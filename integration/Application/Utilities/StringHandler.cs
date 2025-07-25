using System.Text;

namespace integration.Utilities;

public static class StringHandler
{
    public static string SanitizeComment(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        var allowedChars = new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZабвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ0123456789 !*,./\\?");
        
        var sanitized = new StringBuilder();
        foreach (var c in input)
        {
            if (allowedChars.Contains(c))
                sanitized.Append(c);
        }
        
        return sanitized.Length <= 400 
            ? sanitized.ToString() 
            : sanitized.ToString().Substring(0, 400);
    }
}