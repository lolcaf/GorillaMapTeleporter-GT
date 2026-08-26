using System;
using System.IO;
using UnityEngine;

namespace GorillaMapTeleporter.Classes;

/// <summary>
/// Class for writing to logs.
/// </summary>
public class GorillaLog
{
    private readonly StreamWriter _currentWriter;

    /// <summary>
    /// A divider used to seperate notable messages and text from other log messages.
    /// </summary>
    public const string Divider = "======================================================";

    /// <summary>
    /// Log the line into the log.
    /// </summary>
    public void Write(string text, string ending)
    {
        var fmt = $"{text}{ending}";

        _currentWriter.Write(fmt);
        Debug.Log(fmt);
    }

    /// <summary>
    /// Log the line into the log with the logLevel string.
    /// </summary>
    public void WriteLine(string text) => Write($"[{Time.deltaTime}]: {text}", "\n");

    /// <summary>
    /// Log an exception to the file. This logs the message, source, and stack trace of the exception.
    /// </summary>
    public void WriteException(Exception ex)
    {
        WriteLine($"{Divider}\nAn exception occured!");
        WriteLine($"  Message:   {ex.Message}");
        WriteLine($"  Source:    {ex.Source}");
        WriteLine( "  Stack:");
        WriteLine(ex.StackTrace.Replace("\n", "\n\t").Trim().RemoveEnd("\n"));
        WriteLine(Divider);
    }

    // Shortcuts
    public void Write(Exception ex) => WriteException(ex);
    public void WriteLine(Exception ex) => WriteException(ex);

    /// <summary>
    /// Close the log and disable writing.
    /// </summary>
    public void Dispose()
    {
        _currentWriter.Dispose();
    }

    /// <summary>
    /// Create a new GorillaLog.
    /// </summary>
    public GorillaLog()
    {
        var dataPath = Path.Combine(Application.persistentDataPath, Constants.Name);
        Directory.CreateDirectory(dataPath);

        _currentWriter = new StreamWriter(Path.Combine(dataPath, "Latest.log"));
        _currentWriter.AutoFlush = true;
    }
}