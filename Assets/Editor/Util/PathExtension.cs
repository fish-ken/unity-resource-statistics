using System.IO;
using UnityEngine;

public static class PathExtension
{
    /// <summary>
    /// Root path of project
    /// </summary>
    public static string ProjectRootPath => Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    /// <summary>
    /// Library path
    /// </summary>
    public static string LibraryPath => $"{ProjectRootPath}/Library";
}