using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShoppingCartController : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void joinCartServerRpc(NetworkObjectReference parent)
    {
        parent.TryGet(out NetworkObject parentNetworkObject);
        this.GetComponent<NetworkObject>().TrySetParent(parentNetworkObject, false); //.transform.parent.parent
        //this.transform.parent.parent.localPosition = new(0, 0, 3); // y = -0.6F Z = 3
        joinCartClientRpc(parent);
    }

    [ClientRpc]
    private void joinCartClientRpc(NetworkObjectReference parent)
    {
        parent.TryGet(out NetworkObject parentNetworkObject);
        this.GetComponent<NetworkObject>().TrySetParent(parentNetworkObject, false); //transform.parent.parent
        this.transform.localPosition = new(0, 0, 3); // y = -0.6F Z = 3 , transform.parent.parent.
        //this.transform.parent.parent.localRotation = new(0,0,0,0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void unjoinCartServerRpc()
    {
        this.GetComponent<NetworkObject>().TryRemoveParent(); //transform.parent.parent
        unjoinCartClientRpc();
    }

    [ClientRpc]
    private void unjoinCartClientRpc()
    {
        this.transform.parent.parent.GetComponent<NetworkObject>().TryRemoveParent();
    }
}
