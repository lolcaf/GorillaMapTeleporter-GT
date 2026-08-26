using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaMapTeleporter.Utilities;

/// <summary>
/// Utilities for managing and loading objects from Asset Bundles.
/// </summary>
public static class AssetBundleUtilities
{
    // AssetBundleUtilities caches the asset bundles in memory.
    // You can clear bundle cache when you're done with "FreeCache()"
    private static readonly Dictionary<string, AssetBundle> bundleCache = new();

	/// <summary>
    /// Load an asset bundle into the cache, set the provided out AssetBundle to it and return true if it was loaded successfully.
    /// </summary>
    public static bool LoadToCache(string path, out AssetBundle? bundle)
    {
        bundleCache.TryGetValue(path, out var _bundle);
        _bundle ??= AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(path));
        
        if (_bundle == null) {
            Debug.LogError("The asset bundle has failed to load.");
			bundle = null;
            return false;
        }

        bundleCache.TryAdd(path, _bundle);

		bundle = _bundle;
        return true;
    }

    /// <summary>
    /// Load an asset bundle and return the AssetBundle representing it.
    /// </summary>
    public static AssetBundle? LoadAssetBundle(string path)
    {
        if (LoadToCache(path, out var bundle))
            return bundle;
        
        return null;
    }

    /// <summary>
    /// Load an object from an Asset Bundle with the provided name.
    /// </summary>
    public static T Load<T>(string path, string name) where T : Object
	{
        var ab = LoadAssetBundle(path);
		var obj = ab?.LoadAsset<T>(name);

		if (ab == null || obj == null) {
			Debug.LogError($"Cannot load assetbundle \"{path}\" object \"{name}\" to type \"{typeof(T).FullName}.\nValid streams: \n\t{Assembly.GetExecutingAssembly().GetManifestResourceNames().Join("\n\t")}");
            return null;
        }

		return (T)obj;
	}

    /// <summary>
    /// Unload all Asset Bundles inside of the cache and clear the cache.
    /// This should be called after all asset bundles have been loaded.
    /// </summary>
    public static void FreeCache()
    {
        foreach (var ab in bundleCache.Values)
            ab.Unload(false);
        
        bundleCache.Clear();
    }

#region Shortcuts
	/// <summary>
    /// Load a GameObject from an Asset Bundle with the provided name.
    /// </summary>
    public static GameObject Load(string path, string name) => Load<GameObject>(path, name);

    /// <summary>
    /// Load an asset bundle into the cache and return true if it was loaded successfully.
    /// </summary>
    public static bool LoadToCache(string path) => LoadToCache(path, out var _);
#endregion
}
