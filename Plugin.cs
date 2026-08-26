using BepInEx;
using GorillaLocomotion;
using GorillaMapTeleporter.Classes;
using GorillaMapTeleporter.MonoBehaviors;
using GorillaMapTeleporter.Patches;
using GorillaMapTeleporter.Utilities;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilla.Attributes;

namespace GorillaMapTeleporter;

[BepInPlugin(Constants.Guid, Constants.Name, Constants.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
[ModdedGamemode]
public class Plugin : BaseUnityPlugin
{
    /* Current supported maps:
    Forest
    City
    Canyons
    Clouds
    Caves
    Mountain
    Basement
    Metropolis
    Arcade
    Critters
    Skate Park
    Monke Blocks
    Ghost Reactor
    Lava Forest
    Space
    Share My Blocks
    Magmarena
    */

    public static Plugin Instance { get; private set; }

    public static GorillaLog Log = new GorillaLog();

    private bool inModdedLobby = false;

    public bool InValidRoom => !NetworkSystem.Instance.InRoom || (inModdedLobby && !PhotonNetwork.CurrentRoom.IsVisible) && !CustomMapManager.IsLocalPlayerInVirtualStump(); // this should hopefully work

    public GTZone selectedZone = GTZone.none;

    public List<PressableButton> allButtons = new List<PressableButton>();

    private Dictionary<GTZone, Vector3> replacementPositions = new Dictionary<GTZone, Vector3> { // some of the network trigger positions are bad so these are replacements
        { GTZone.canyon, new Vector3(-86, 11, -109) },
        { GTZone.skyJungle, new Vector3(-77, 164, -98) },
        { GTZone.basement, new Vector3(-35.5f, 14.75f, -89.1f) },
        { GTZone.Metropolis, new Vector3(63, 4, -240) },
        { GTZone.arcade, new Vector3(-31.7f, 25.5f, -100f) },
        { GTZone.arena, new Vector3(100, 5, 200 ) },
        { GTZone.monkeBlocksShared, new Vector3(-282, 31.5f, -223.5f) }
    };

    private void Awake()
    {
        Instance = this;
        HarmonyPatches.Patch();
        Log.WriteLine("Awake - Gorilla Map Teleporter");
        GorillaTagger.OnPlayerSpawned(() => MethodUtilities.Attempt(OnPlayerSpawned));
    }

    private void OnPlayerSpawned()
    {
        Log.WriteLine("OnPlayerSpawned function called");
        try
        {
            GameObject menu = AssetBundleUtilities.Load("GorillaMapTeleporter.Resources.AssetBundle.gorillamapteleporter", "TeleportMenu");
            Instantiate(menu).AddComponent<MenuManager>();
            Log.WriteLine("Successfully loaded assetbundle");
        }
        catch (Exception e)
        {
            Log.WriteException(e);
        }
        GameObject quitBox = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
        Destroy(quitBox.GetComponent<GorillaQuitBox>());
        quitBox.AddComponent<NewQuitBox>();
    }

    public void TeleportToZone(GTZone zone)
    {
        if (!InValidRoom) return;
        GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.allJoinTriggers.FirstOrDefault(t => t.zone == zone);
        ZoneManagement.SetActiveZone(zone);
        trigger?.OnBoxTriggered();
        if (replacementPositions.TryGetValue(zone, out Vector3 position))
        {
            TeleportPlayer(position);
        }
        else
        {
            TeleportPlayer(trigger?.transform.position ?? VRRig.LocalRig.transform.position);
        }
        zone = GTZone.none;
        RefreshButtonGlow();
    }

    private void TeleportPlayer(Vector3 position)
    {
        GTPlayer.Instance.TeleportTo(position - GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.transform.position, GTPlayer.Instance.transform.rotation);
        VRRig.LocalRig.transform.position = position;
    }

    public void SelectZone(GTZone zone)
    {
        selectedZone = zone;
        RefreshButtonGlow();
    }

    public void RefreshButtonGlow()
    {
        foreach (PressableButton button in allButtons)
        {
            if (button.connectedZone == GTZone.none) continue;
            button.transform.Find("SelectGlow").gameObject.SetActive(button.connectedZone == selectedZone);
        }
    }

    [ModdedGamemodeJoin]
    private void RoomJoined()
    {
        inModdedLobby = true;
    }

    [ModdedGamemodeLeave]
    private void RoomLeft()
    {
        inModdedLobby = false;
    }
}
