using UnityEngine;

namespace GorillaMapTeleporter.MonoBehaviors;

public class NewQuitBox : MonoBehaviour // This mod poses a risk to falling out of the map so I decided to replace it with a better one
{
    private void Start()
    {
        gameObject.layer = 18;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GorillaTagger.Instance.rightHandTriggerCollider || other.gameObject == GorillaTagger.Instance.leftHandTriggerCollider)
        {
            Plugin.Instance.TeleportToZone(GTZone.forest);
        }
    }
}
