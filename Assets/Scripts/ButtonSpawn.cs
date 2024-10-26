using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class ButtonSpawn : NetworkBehaviour
    {
        [SerializeField] private ObjectListSO _objectListSO;
        [SerializeField] private int _objectListSOIndex;

        public int ObjectListSOIndex
        {
            get { return _objectListSOIndex; }
        }

        public ObjectListSO ObjectListSO
        {
            get { return _objectListSO; }
        }


        public void Activate()
        {
            SpawnObject();
        }

        public void SpawnObject()
        {
            SpawnObjectServerRpc(_objectListSOIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnObjectServerRpc(int objectListSOIndex)
        {
            ObjectSO obj = _objectListSO.objects[objectListSOIndex];
            //GameObject shoppingCart = GameObject.Find("ShoppingCart");//
            Transform objTransform = Instantiate(obj.prefab);//,shoppingCart.GetComponent<Transform>()
            NetworkObject objNetworkObject = objTransform.GetComponent<NetworkObject>();
            //objNetworkObject.GetComponent<Transform>().localScale = new(0.3f, 0.3f, 0.3f);
            
            objNetworkObject.Spawn();
            SetNetworkObjectParent(objNetworkObject, GameObject.Find("ShoppingCart").GetComponent<NetworkObject>());//Products
            SpawnObjectClientRpc(objNetworkObject);
        }

        private void SetNetworkObjectParent(NetworkObject objNetworkObject, NetworkObject networkObjectParent)
        {
            objNetworkObject.TrySetParent(networkObjectParent, false);
            objNetworkObject.transform.localPosition = networkObjectParent.transform.localPosition;
            objNetworkObject.gameObject.layer = 0;

        }

        [ClientRpc]
        public void SpawnObjectClientRpc(NetworkObjectReference objNetworkObject)
        {
            objNetworkObject.TryGet(out NetworkObject netObj);
            SetNetworkObjectParent(netObj, GameObject.Find("ShoppingCart").GetComponent<NetworkObject>());
        }


    }
}
