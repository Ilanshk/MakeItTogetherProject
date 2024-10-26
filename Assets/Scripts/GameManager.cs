using Assets.Scripts.GameFramework.Manager;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private int _numOfPlayers;
    public int NumOfPlayers
    {
        get { return _numOfPlayers; }
    }
    private NetworkVariable<bool>[] _checkpoints = new NetworkVariable<bool>[3] {new(false), new(false), new(false)};
    public NetworkVariable<bool>[] Checkpoints
    {
        get => _checkpoints;
        set => _checkpoints = value;
    }
    
    [SerializeField] public NetworkVariable<int> _currNum = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> _totalSum = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public TextMeshProUGUI _resNum;
    [SerializeField] public TextMeshProUGUI _resNumForTeacher;

    public NetworkVariable<int> TotalSum
    {
        get => _totalSum;
        set => _totalSum.Value = value.Value;
    }
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        if(RelayManager.Instance.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
            (byte[] allocationId, byte[] key, byte[] connectionData, string ip, int port) = RelayManager.Instance.GetHostConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ip, (ushort)port, allocationId, key, connectionData, true);
            //_numOfPlayers = GameObject.Find("FirstPlayerObj").GetComponent<PlayerObjectData>().NumberOfPlayers;
            NetworkManager.Singleton.StartHost(); 
        }
        else
        {
            (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ip, int port) = RelayManager.Instance.GetClientConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ip, (ushort)port, allocationId, key, connectionData, hostConnectionData,true);
            //_numOfPlayers = GameObject.Find("SecondPlayerObj").GetComponent<PlayerObjectData>().NumberOfPlayers;
            NetworkManager.Singleton.StartClient();
        }
        

    }

    private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Pending = false;
    }

    private bool GameCheck(int num)
    {
        if(num >= _totalSum.Value)
        {
            return true;
        }
        return false;
    }

    public bool CheckFirstStage(Dictionary<string, string> itemList)
    {
        bool check = true;
        foreach(KeyValuePair<string, string> item in itemList)
        {
            if(int.Parse(item.Value) != 0)
            {
                check = false;
                break;
            }
        }
        return check;
    }


    [ServerRpc(RequireOwnership = false)]
    public void GameManagerWorkServerRpc(int num)
    {
        _currNum.Value += num;
        WriteToUIClientRpc(_currNum.Value);
    }

    [ClientRpc]
    private void WriteToUIClientRpc(int num)
    {
        if (num == _totalSum.Value)
        {
            _checkpoints[2].Value = true;
        }
        _resNum.text = num.ToString() + "/" + _totalSum.Value.ToString();
        _resNumForTeacher.text = num.ToString() + "/" + _totalSum.Value.ToString();
        if (GameCheck(num))
        {
            GameObject.Find("SecondPlayerObj").GetComponent<StudentScript>().RemainAmount = 0;
            _checkpoints[1].Value = true;
        }
        
        
        
    }

    public bool MoveToLastCanva()
    {
        if (_checkpoints[0].Value == true && _checkpoints[1].Value == true && _checkpoints[2].Value == false)
            return true;
        return false;
    }

    public bool EndGame()
    {
        if (_checkpoints[0].Value == true && _checkpoints[1].Value == true && _checkpoints[2].Value == true)
            return true;
        return false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void GameExitServerRpc()
    {
        NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MakeZeroCurrentNumServerRpc()
    {
        _currNum.Value = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTotalSumServerRpc(int sum)
    {
        TotalSum.Value = sum;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetUpUIServerRpc()
    {
        SetUpUIClientRpc();
    }
    [ClientRpc]
    public void SetUpUIClientRpc()
    {
        _resNum.text = _currNum.Value.ToString() + "/" + _totalSum.Value.ToString();
        _resNumForTeacher.text = _currNum.Value.ToString() + "/" + _totalSum.Value.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCurrNumServerRpc(int num)
    {
        _currNum.Value = num;
    }


}
