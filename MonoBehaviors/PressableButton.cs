using System;
using UnityEngine;

namespace GorillaMapTeleporter.MonoBehaviors;

public class PressableButton : MonoBehaviour // this script is usable for a normal button or a map select button
{
    public Action pressed;

    public GTZone connectedZone = GTZone.none;

    private float cooldownTime;

    private void Start()
    {
        gameObject.layer = 18;
        Plugin.Instance.allButtons.Add(this);
    }

    private void OnButtonPressed(bool leftHand)
    {
        pressed?.Invoke();
        VRRig.LocalRig.PlayHandTapLocal(67, leftHand, 0.4f);
        if (connectedZone != GTZone.none)
        {
            Plugin.Instance.SelectZone(connectedZone);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Plugin.Log.WriteLine("Trigger entered with: " + other.gameObject.name);
        if ((other.gameObject == GorillaTagger.Instance.rightHandTriggerCollider || other.gameObject == GorillaTagger.Instance.leftHandTriggerCollider) && Time.time > cooldownTime)
        {
            cooldownTime = Time.time + 0.4f;
            OnButtonPressed(other.gameObject == GorillaTagger.Instance.leftHandTriggerCollider);
        }
    }
}
