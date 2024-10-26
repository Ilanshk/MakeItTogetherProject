using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NtwObject : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ObjectSO objectSO;

    public ObjectSO GetObjectSO()
    {
        return objectSO;
    }

    /*public override void OnNetworkObjectParentChanged(NetworkObject parent)
    {
        if(IsClient || NetworkManager.ConnectedClients.Count == 1)
        {
            transform.localPosition = parent.transform.localPosition;
            transform.localScale = new(0.5f, 0.5f, 0.5f);
        }
        Debug.Log("Changing Position");
    }*/

}
