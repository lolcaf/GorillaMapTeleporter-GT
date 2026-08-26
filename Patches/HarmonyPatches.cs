using GorillaLocomotion;
using HarmonyLib;

namespace GorillaMapTeleporter.Patches;

public static class HarmonyPatches
{
    // For information on how to use Harmony, view this documentation:
    // https://harmony.pardeike.net/articles/intro.html

    [HarmonyPatch(typeof(GTPlayer))]
    public static class ExamplePatch
    {
        [HarmonyPatch("Awake", MethodType.Normal)]
        [HarmonyPostfix]
        private static void AwakePatch(GTPlayer __instance)
        {
            Plugin.Instance.Log($"[{Constants.Name}]: hello world!");
        }
    }


    /// Here is where the actual patching starts! You don't need to mess with any of this.

    private static HarmonyLib.Harmony? _harmonyInstance;

    /// <summary>
    ///     The current instance of Harmony that is patching the assembly.
    ///     If there is no Harmony instance, it will create one and return it.
    ///     You do not need to touch this section
    /// </summary>
    private static HarmonyLib.Harmony HarmonyInstance
    {
        get
        {
            _harmonyInstance ??= new HarmonyLib.Harmony(Constants.Guid);
            return _harmonyInstance;
        }
    }

    /// <summary>
    /// Patch the assembly.
    /// </summary>
    public static void Patch()
    {
        HarmonyInstance.PatchAll();
    }

    /// <summary>
    /// Unpatch the assembly.
    /// </summary>
    public static void Unpatch()
    {
        HarmonyInstance.UnpatchSelf();
    }
}
