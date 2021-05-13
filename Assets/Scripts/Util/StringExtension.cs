using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Extension for string
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Return true if string is null or empty
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string input)
    {
        return string.IsNullOrEmpty(input);
    }
    
    /// <summary>
    /// Creates an array of strings by splitting this string at each occurence of a separator
    /// </summary>
    /// <param name="input"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static string[] Split(this string input, string pattern)
    {
        return Regex.Split(input, pattern);
    }

    /// <summary>
    /// Make string's first character to upper-character
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToUpperFirstChar(this string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException($"{nameof(input)} cannot be null or empty", nameof(input));

        return input.First().ToString().ToUpper() + input.Substring(1);
    }

    #region Escape / Unescape

    /// <summary>
    /// Escape string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToEscape(this string input)
    {
        return Regex.Escape(input);
    }
    
    /// <summary>
    /// Unescape string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToUnescape(this string input)
    {
        return Regex.Unescape(input);
    }

    #endregion

    #region Byte

    /// <summary>
    /// Byte suffixes 
    /// </summary>
    private readonly static string[] sizeSuffixList = { "bytes", "kb", "mb", "gb", "tb", "pb", "eb", "zb", "yb" };

    /// <summary>
    /// Convert byte size to string with suffix
    /// </summary>
    /// <param name="size">Byte size</param>
    /// <param name="decimalPlaces">Decimal place</param>
    /// <returns></returns>
    private static string ToByteStringWithSuffix(this long size, int decimalPlaces = 1)
    {
        if (decimalPlaces < 0)
            throw new ArgumentOutOfRangeException("decimalPlaces");
        if (size < 0) 
            return $"- {ToByteStringWithSuffix(-size)}";
        if (size == 0)  
            return string.Format("{0:n" + decimalPlaces + "} bytes", 0); 

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(size, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)size / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            sizeSuffixList[mag]);
    }

    #endregion
}
