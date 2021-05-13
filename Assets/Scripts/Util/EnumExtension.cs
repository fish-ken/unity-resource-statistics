using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension class for enum
/// </summary>
/// <typeparam name="T"></typeparam>
public static class EnumExtension<T> where T : struct, IConvertible
{
    /// <summary>
    /// Return true when T is enum type
    /// </summary>
    public static bool IsEnum
    {
        get => typeof(T).IsEnum;
    }

    /// <summary>
    /// Get enum`s count
    /// </summary>
    public static int Count
    {
        get
        {
            if (IsEnum == false)
                return 0;

            return Enum.GetValues(typeof(T)).Length;
        }
    }

    /// <summary>
    /// Get enumeration of T
    /// </summary>
    public static IEnumerable<T> Enumerable
    {
        get
        {
            if (IsEnum == false)
                return null;

            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>
    /// Convert string to enum
    /// </summary>
    /// <param name="input">string to convert with enum</param>
    /// <returns></returns>
    public static T Parse(string input)
    {
        return (T)Enum.Parse(typeof(T), input);
    }

    /// <summary>
    /// Get enum list of T
    /// </summary>
    public static List<T> List
    {
        get
        {
            if (IsEnum == false)
                return null;

            return Enumerable?.ToList();
        }
    }

    /// <summary>
    /// Get all enum`s name
    /// </summary>
    public static string[] Names
    {
        get
        {
            if (IsEnum == false)
                return null;

            return Enum.GetValues(typeof(T)).Cast<T>().Select(elem => elem.ToString()).ToArray();
        }
    }
}