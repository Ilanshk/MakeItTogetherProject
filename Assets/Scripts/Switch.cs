using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class Switch : NetworkBehaviour
    {
        private NetworkVariable<bool> _isActive = new NetworkVariable<bool>();

        public NetworkVariable<bool> IsActive { get { return _isActive; } }
        private string _name;
        private Collider _playerCollider;
        public delegate void SwitchChanged(Switch doorSwitch, bool isActive);
        public event SwitchChanged OnSwitchChanged;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _isActive.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(bool wasActive, bool isActive)
        {
            if(isActive && GameObject.Find("GameManager").GetComponent<GameManager>().Checkpoints[0].Value && _playerCollider.transform.Find("ShoppingCart").name == "ShoppingCart")
            {
                Debug.Log("IsActive");               
                if (_name == "Student")
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>()._studentCanvas.SetActive(true);
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>()._studentMovementCanvas.SetActive(false);
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>().EnableStudentCanvaClientRpc(true, _playerCollider.gameObject.GetComponentInParent<NetworkObject>());

                }

            }
            else
            {
                Debug.Log("IsNotActive");
                if (_name == "Student")
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>()._studentCanvas.SetActive(false);
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>()._studentMovementCanvas.SetActive(true);
                    _playerCollider.gameObject.GetComponentInParent<PlayerController>().EnableStudentCanvaClientRpc(false, _playerCollider.gameObject.GetComponentInParent<NetworkObject>());

                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _name = other.gameObject.GetComponentInParent<PlayerController>().PlayerName.text;
            _playerCollider = other;
            OnSwitchChangedServerRpc(true);
        }

        private void OnTriggerExit(Collider other)
        {
            _name = other.gameObject.GetComponentInParent<PlayerController>().PlayerName.text;
            _playerCollider = other;
            OnSwitchChangedServerRpc(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnSwitchChangedServerRpc(bool isActive)
        {
            _isActive.Value = isActive;
            OnSwitchChanged?.Invoke(this, isActive);
        }

    }
}
