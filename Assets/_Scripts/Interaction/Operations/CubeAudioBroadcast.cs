using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Broadcast;
using FishNet;
using FishNet.Connection;
using FishNet.Transporting;
using System;
using CallSOS.Player.Interaction;
using CallSOS.Utilities;

//Made by Bobsi Unity - for Youtube
public class CubeAudioBroadcast : MonoBehaviour, IRaycastable
{
    public List<Transform> cubePositions = new List<Transform>();
    public int transformIndex;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<PositionIndex>(OnPositionBroadcast);
        //InstanceFinder.ServerManager.RegisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<PositionIndex>(OnPositionBroadcast);
        //InstanceFinder.ServerManager.UnregisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int nextIndex = transformIndex + 1;
            if (nextIndex >= cubePositions.Count)
                nextIndex = 0;

            if (InstanceFinder.IsServerStarted)
            {
                Debug.Log("IsServer");
                InstanceFinder.ServerManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
            else if (InstanceFinder.IsClientStarted)
            {
                Debug.Log("IsClient");
                InstanceFinder.ClientManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
        }

        transform.position = cubePositions[transformIndex].position;
    }

    private void OnPositionBroadcast(PositionIndex indexStruct, Channel channel)
    {
        Debug.Log("OnPositionBroadcast");
        transformIndex = indexStruct.tIndex;
    }

    private void OnClientPositionBroadcast(NetworkConnection networkConnection, PositionIndex indexStruct, Channel channel)
    {
        Debug.Log("OnClientPositionBroadcast");
        InstanceFinder.ServerManager.Broadcast(indexStruct);
    }

    public CursorType GetCursorType()
    {
        return CursorType.Point;
    }

    public bool CanHandleRaycast(ObjectInteractController callingController)
    {
        return true;
    }

    public InteractItem GetInteractItem()
    {
        return GetComponent<InteractItem>();
    }

    public struct PositionIndex : IBroadcast
    {
        public int tIndex;
    }
}