using System;
using UnityEngine;

namespace GorillaMapTeleporter.Utilities;

/// <summary>
/// Utilities for working with methods.
/// </summary>
public static class MethodUtilities
{
    /// <summary>
    /// Returns true if the provided Action caused no errors in execution.
    /// </summary>
    public static bool Attempt(Action method)
    {
        try { method(); }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }

        return true;
    }
}