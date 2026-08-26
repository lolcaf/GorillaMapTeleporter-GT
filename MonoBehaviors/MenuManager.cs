using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaMapTeleporter.MonoBehaviors;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private GameObject offsetGO;

    private GameObject page1;

    private GameObject page2;

    private float menuToggleCooldown;

    private bool menuLocked = false;

    private void Start()
    {
        Instance = this;
        offsetGO = transform.Find("Offset").gameObject;
        page1 = offsetGO.transform.Find("Page1").gameObject;
        page2 = offsetGO.transform.Find("Page2").gameObject;
        Plugin.Log.WriteLine("MenuManager Added");
        offsetGO.transform.Find("Version").gameObject.GetComponent<TextMeshPro>().text = "Version: " + Constants.Version;
        offsetGO.transform.localPosition = new Vector3(0, -0.1f, 0.6f); // fixed the offset, I dont wanna build the asset bundle again
        page1.SetActive(true);
        page2.SetActive(false);
        SetupButtons();
        offsetGO.SetActive(false);
    }

    private void Update()
    {
        if (!Plugin.Instance.InValidRoom)
        {
            offsetGO.SetActive(false);
            return;
        }
        if (ControllerInputPoller.instance.rightControllerPrimaryButton && Time.time > menuToggleCooldown && menuLocked == false)
        {
            menuToggleCooldown = Time.time + 0.4f;
            offsetGO.SetActive(!offsetGO.activeSelf);
        }
        if (offsetGO.activeSelf)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void SetupButtons()
    {
        offsetGO.transform.Find("Next").AddComponent<PressableButton>().pressed += Next;
        offsetGO.transform.Find("Previous").AddComponent<PressableButton>().pressed += Previous;
        offsetGO.transform.Find("Teleport").AddComponent<PressableButton>().pressed += TeleportButton;

        ConnectButton("Forest", 1, GTZone.forest);
        ConnectButton("City", 1, GTZone.city);
        ConnectButton("Canyon", 1, GTZone.canyon);
        ConnectButton("Cloud", 1, GTZone.skyJungle);
        ConnectButton("Cave", 1, GTZone.cave);
        ConnectButton("Mountain", 1, GTZone.mountain);
        ConnectButton("Basement", 1, GTZone.basement);
        ConnectButton("Metro", 1, GTZone.Metropolis);
        ConnectButton("Arcade", 1, GTZone.arcade);
        ConnectButton("Critters", 1, GTZone.critters);

        ConnectButton("SkatePark", 2, GTZone.hoverboard);
        ConnectButton("MonkeBlocks", 2, GTZone.monkeBlocks);
        ConnectButton("GhostReactor", 2, GTZone.ghostReactor);
        ConnectButton("LavaForest", 2, GTZone.VIMExperience1); // this is free now
        ConnectButton("Space", 2, GTZone.spaceMap);
        ConnectButton("ShareMyBlocks", 2, GTZone.monkeBlocksShared);
        ConnectButton("MagmArena", 2, GTZone.arena); // this map was removed (map doesnt work and textures are broken but that aint my problem)
    }

    private void ConnectButton(string buttonName, int page, GTZone zone)
    {
        GameObject button;
        if (page == 1)
        {
            button = page1.transform.Find(buttonName + "Button").gameObject;
        }
        else // only two pages
        {
            button = page2.transform.Find(buttonName + "Button").gameObject;
        }
        button.AddComponent<PressableButton>().connectedZone = zone;
    }

    private void TeleportButton()
    {
        StartCoroutine(TeleportButtonRoutine());
    }

    private IEnumerator TeleportButtonRoutine()
    {
        if (Plugin.Instance.selectedZone == GTZone.none)
        {
            offsetGO.transform.Find("Teleport").Find("Text").GetComponent<TextMeshPro>().text = "<color=red>You have no map selected!</color>";
            yield return new WaitForSeconds(2);
            offsetGO.transform.Find("Teleport").Find("Text").GetComponent<TextMeshPro>().text = "Go!";
        }
        else
        {
            Plugin.Instance.TeleportToZone(Plugin.Instance.selectedZone);
        }
    }

    private void Next()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        Plugin.Instance.RefreshButtonGlow();
    }

    private void Previous()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        Plugin.Instance.RefreshButtonGlow();
    }
}
