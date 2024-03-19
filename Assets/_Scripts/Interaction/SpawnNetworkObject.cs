using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNetworkObject : NetworkBehaviour
{
    public GameObject objectToSpawnPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnObjectServerRPC();
        }    
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectServerRPC()
    {
        GameObject newObject = Instantiate(objectToSpawnPrefab, transform.position, Quaternion.identity);
        ServerManager.Spawn(newObject);
        Destroy(gameObject);
    }
}
