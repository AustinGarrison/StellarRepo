using CallSOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SOSTeleporter : MonoBehaviour
{
    public SOSTeleporter TeleportTo;

    public UnityAction<PlayerControllerLocal> OnCharacterTeleport;

    public bool isBeingTeleportedTo { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBeingTeleportedTo)
        {
            PlayerControllerLocal cc = other.GetComponent<PlayerControllerLocal>();

            if (cc)
            {
                Debug.Log("Telport Position: " + TeleportTo.transform.position);
                Debug.Log("Telport Rotation: " + TeleportTo.transform.rotation);

                cc.motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);

                if (OnCharacterTeleport != null)
                {
                    OnCharacterTeleport(cc);
                }
                TeleportTo.isBeingTeleportedTo = true;
            }
        }

        isBeingTeleportedTo = false;
    }
    
}