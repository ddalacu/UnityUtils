using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

public static class StringExtensions
{
    [Pure]
    public static int CountCharacter(this string text, char character)
    {
        if (text == null)
            return 0;

        int count = 0;
        int textLength = text.Length;
        for (int i = 0; i < textLength; i++)
        {
            if (text[i] == character)
                count++;
        }

        return count;
    }

    public static int CountCharsNoSpaces(this string text)
    {
        int count = 0;
        var textLength = text.Length;
        for (int i = 0; i < textLength; i++)
            if (text[i] != ' ')
                count++;

        return count;
    }

    [Pure]
    public static long GetInt64Hash(this string text)
    {
        unchecked
        {
            long hash = 23;
            foreach (char c in text)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    [Pure]
    public static int GetInt32Hash(this string text)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in text)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    [Pure]
    public static bool GetResourcesFormatedPath(ref string path)
    {
        const string ResourcesString = "Resources";
        const int ResourcesStringLength = 9;

        int index = path.LastIndexOf(ResourcesString, StringComparison.InvariantCulture);

        if (index == -1)
        {
            return false;
        }

        index += ResourcesStringLength + 1;
        path = path.Remove(0, index);
        path = Path.GetFileNameWithoutExtension(path);
        return true;
    }

    [Pure]
    public static string Base64Encode([NotNull] string plainText)
    {
        if (plainText == null)
            throw new ArgumentNullException(nameof(plainText));

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    [Pure]
    public static string Base64Decode([NotNull] string base64EncodedData)
    {
        if (base64EncodedData == null)
            throw new ArgumentNullException(nameof(base64EncodedData));

        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    [Pure]
    public static bool IsValidEmailFormat([NotNull]this string str)
    {
        if (string.IsNullOrEmpty(str) || !str.Contains("@"))
        {
            return false;
        }
        try
        {
            System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(str);
            return str.Equals(addr.Address);
        }
        catch
        {
            return false;
        }
    }

}
