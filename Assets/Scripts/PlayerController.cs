using Cinemachine;
using Game;
using GameFramework.Network.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI.TableUI;
using Unity.Networking;
using UnityEngine.SceneManagement;
using Unity.Services.Vivox;
using Unity.Services.Authentication;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{

    [SerializeField] private Vector2 _minMaxRotation;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private NetworkMovementComponent _playerMovement;
    [SerializeField] private float _interactDistance;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private GameObject _switcher;
    [SerializeField] public GameObject _teacherCanvas;
    [SerializeField] public GameObject _studentCanvas;
    [SerializeField] public GameObject _studentCanvasList;
    [SerializeField] public GameObject _studentMovementCanvas;
    [SerializeField] public TextMeshProUGUI _studentResult;
    [SerializeField] public TextMeshProUGUI _teacherResult;
    [SerializeField] private AudioListener _audioListener;

    public TextMeshPro PlayerName
    {
        get { return _playerName; }
    }

    private CharacterController _cc;
    private PlayerControl _playerControl;
    private float _cameraAngle;

    private void Awake()
    {
        
    }

    [ClientRpc]
    public void EnableStudentCanvaClientRpc(bool active, NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject player);
        if (this.gameObject.GetComponent<NetworkObject>().OwnerClientId == player.OwnerClientId && IsClient)
        {
            if(active)
                Cursor.lockState = CursorLockMode.Confined;
            else
                Cursor.lockState = CursorLockMode.Locked;
            player.gameObject.GetComponent<PlayerController>()._studentCanvas.SetActive(active);
            player.gameObject.GetComponent<PlayerController>()._studentMovementCanvas.SetActive(!active);
        }
            
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"I'm spawning!!!!! {IsHost}");
        CinemachineVirtualCamera cvm = _camTransform.gameObject.GetComponent<CinemachineVirtualCamera>();
        _switcher = GameObject.Find("SwitchRight");
        
        if (IsOwner)
        {
            cvm.Priority = 1;
        }
        else
        {
            cvm.Priority = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _cc = GetComponent<CharacterController>();
        _switcher = GameObject.Find("Switch");
        _playerControl = new PlayerControl();
        _playerControl.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerPropertiesServerRpc();

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 movementInput = _playerControl.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = _playerControl.Player.Look.ReadValue<Vector2>();
        if (_switcher.GetComponent<Switch>().IsActive.Value == true)
            lookInput = new Vector2(0, 0);
        
        if (IsClient && IsLocalPlayer)
        {
            _playerMovement.ProcessLocalPlayerMovement(movementInput, lookInput);
            
        }
        else
        {
            _playerMovement.ProcessSimulatedPlayerNovement();
        }


        if(IsLocalPlayer && _playerControl.Player.Interact.WasPerformedThisFrame())
        {
            if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
            {

                if (hit.collider.TryGetComponent<ButtonDoor>(out ButtonDoor buttonDoor))
                {
                    UseButtonServerRpc();
                }
                else if (hit.collider.TryGetComponent<ButtonSpawn>(out ButtonSpawn buttonSpawn))
                {
                    UseSpawnButtonRpc();
                    
                }   
                else if(hit.collider.TryGetComponent<ShoppingCartController>(out ShoppingCartController shoppingCart))
                {
                    InteractWithCart();
                }
            }
        }

            
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerPropertiesServerRpc()
    {

        IReadOnlyDictionary<ulong, NetworkClient> networkClients = NetworkManager.ConnectedClients;
        string temp = GameObject.Find("FirstPlayerObj").GetComponent<StudentScript>().PlayerName;
        string temp2 = GameObject.Find("StudentData").GetComponent<StudentScript>().PlayerName;
        if(networkClients.Count > 1)
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in networkClients)
            {
                ulong id = client.Key;
                string temp3 = "";
                NetworkClient cl = NetworkManager.ConnectedClients[id];
                if (this == cl.PlayerObject.gameObject.GetComponent<PlayerController>())
                {
                    if (OwnerClientId == 0)
                    {
                        this._playerName.text = temp;
                        temp3 = temp;
                        
                    }
                    else
                    {
                        this._playerName.text = temp2;
                        temp3 = temp2;
                    }

                }
                string channelName = NetworkManager.ConnectedClientsIds[0].ToString() + NetworkManager.ConnectedClientsIds[1].ToString();
                SetPlayerPropertiesClientRpc(cl.PlayerObject, temp3, 2, channelName);
            }
        }
        else
        {
            NetworkClient cl = NetworkManager.ConnectedClients[0];
            SetPlayerPropertiesClientRpc(cl.PlayerObject, temp2, 1);
        }
        
        

    }

    [ClientRpc(RequireOwnership = false)]
    private void SetPlayerPropertiesClientRpc(NetworkObjectReference client, string name, int numOfClients, string channelName = null)
    {
        client.TryGet(out NetworkObject obj);
        if (this == obj.gameObject.GetComponent<PlayerController>())
        {
            this._playerName.text = name;
            if(this._playerName.text == "Teacher" && IsLocalPlayer && IsHost)
            {
                Cursor.lockState = CursorLockMode.Confined;
                this._teacherCanvas.SetActive(true);
                GameObject.Find("GameManager").GetComponent<GameManager>()._resNumForTeacher = _teacherResult;
                this._studentCanvasList.SetActive(false);
            }
            else if(this._playerName.text == "Student" && IsLocalPlayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (!IsHost || numOfClients != 2)
                {
                    this._studentCanvasList.SetActive(true);
                    GameObject.Find("GameManager").GetComponent<GameManager>()._resNum = _studentResult;
                    TableUI listOfPurchases = this._studentCanvasList.GetComponentInChildren<TableUI>();
                    Dictionary<string, string> dataForListUnsuited = GameObject.Find("StudentData").GetComponent<StudentScript>().ListOfPurchasesTemp;
                    var dataForListSuited = dataForListUnsuited.Select(x => Tuple.Create(x.Key, x.Value)).ToList();
                    listOfPurchases.Rows = dataForListUnsuited.Count + 1;
                    listOfPurchases.bodyCellProperties.AutoSize = true;
                    for (int i = 0; i < listOfPurchases.Rows - 1; i++)
                    {
                        listOfPurchases.GetCell(i + 1, 0).GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
                        listOfPurchases.GetCell(i + 1, 0).GetComponent<TextMeshProUGUI>().text = dataForListSuited[i].Item1;
                        listOfPurchases.GetCell(i + 1, 1).GetComponent<TextMeshProUGUI>().text = dataForListSuited[i].Item2;
                    }
                    if(dataForListSuited.Count == 0)
                    {
                        _studentCanvasList.SetActive(false);
                    }
                }
            }
            if (IsOwner)
            {
                _audioListener.enabled = true;
                if(Application.HasUserAuthorization(UserAuthorization.Microphone) && numOfClients == 2)
                {
                    VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);
                }
                    
            }
            

        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void UseButtonServerRpc()
    {
        if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            if (hit.collider.TryGetComponent<ButtonDoor>(out ButtonDoor buttonDoor))
            {
                buttonDoor.Activate();
            }
        }
    }

    private void UseSpawnButtonRpc()
    {
        if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            if (hit.collider.TryGetComponent<ButtonSpawn>(out ButtonSpawn buttonSpawn))
            {
                bool check = false;
                Dictionary<string, string> dataForListUnsuited = GameObject.Find("StudentData").GetComponent<StudentScript>().ListOfPurchasesTemp;
                foreach(KeyValuePair<string, string> item in dataForListUnsuited)
                {
                    int amount = Int32.Parse(item.Value);
                    if (buttonSpawn.ObjectListSO.objects[buttonSpawn.ObjectListSOIndex].objectName == item.Key)
                    {
                        if(amount != 0)
                        {
                            buttonSpawn.SpawnObjectServerRpc(buttonSpawn.ObjectListSOIndex);
                            amount--;
                            dataForListUnsuited[item.Key] = amount.ToString();
                            GameObject.Find("StudentData").GetComponent<StudentScript>().ListOfPurchasesTemp = dataForListUnsuited;
                            string productName = buttonSpawn.ObjectListSO.objects[buttonSpawn.ObjectListSOIndex].objectName;
                            _studentCanvasList.GetComponent<StudentListManager>().EditList(productName);
                            check = true;
                            break;
                        }
                        else
                        {
                            _studentCanvasList.GetComponent<StudentListManager>().MessageType(1);
                        }
                        
                    }
                }
                if (!check)
                    _studentCanvasList.GetComponent<StudentListManager>().MessageType(0);
                else 
                {
                    bool success = GameObject.Find("GameManager").GetComponent<GameManager>().CheckFirstStage(dataForListUnsuited);
                    if(success)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().Checkpoints[0].Value = true;
                        _studentCanvasList.GetComponent <StudentListManager>().MessageType(3);
                        _studentCanvasList.GetComponent<StudentListManager>().TurnOffCanva();
                        _studentMovementCanvas.SetActive(true);
                    }
                }
            }
        }
    }


    private void InteractWithCart()
    {
        if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _interactDistance, _interactionLayer))
        {
            if (hit.collider.TryGetComponent<ShoppingCartController>(out ShoppingCartController shoppingCart))
            {
                if(shoppingCart.transform.IsChildOf(this.transform))
                {
                    shoppingCart.unjoinCartServerRpc();
                }
                else
                {
                    shoppingCart.joinCartServerRpc(this.GetComponent<NetworkObject>());
                }
            }
        }
    }
}
