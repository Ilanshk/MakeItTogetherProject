using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine.Rendering;
using UnityEngine;
using Assets.Scripts.GameFramework.Core;
using Assets.Scripts.GameFramework.Events;
using Game;
using Game.Data;

namespace Assets.Scripts.GameFramework.Manager
{
    public class LobbyManager : Singleton<LobbyManager>
    {

        private Lobby _lobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;
        private List<string> _joinedLobbiesId;


        public async Task<bool> HasActiveLobbies()
        {
            _joinedLobbiesId = await LobbyService.Instance.GetJoinedLobbiesAsync();
            if(_joinedLobbiesId.Count > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player,
            };
            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            }
            catch(System.Exception)
            {
                return false;
            }

            Debug.Log($"Lobby created with lobbyID: {_lobby.Id}");
            _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id, 6f));
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));

            return true;
        }

        

        public string GetHostId()
        {
            return _lobby.HostId;
        }

        public async Task<bool> JoinLobby(string code, Dictionary<string, string> data)
        {;
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(data));
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
            {
                Player = player
            };
            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            catch (System.Exception)
            {
                return false;
            }
            Debug.Log($"Joined lobby with lobbyID: {_lobby.Id}");
            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            return true;
        }

        

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyID, float waitTimeSeconds)
        {
            while(true)
            {
                Debug.Log("Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyID);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyID, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyID);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                    LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
                }
                    
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string , PlayerDataObject> playerData = new Dictionary<string , PlayerDataObject>();
            foreach (var (key, value) in data)
            {
                playerData.Add(key, new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: value
                    ));
            }
            return playerData;
        }

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            foreach (var (key, value) in data)
            {
                lobbyData.Add(key, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: value
                    ));
            }
            return lobbyData;
        }
        public void OnApplicationQuit()
        {
            if(_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
        {
            List<Dictionary<string , PlayerDataObject>> data = new List<Dictionary<string , PlayerDataObject>>();

            foreach(Player player in _lobby.Players)
            {
                data.Add(player.Data);
            }
            return data;
        }

        public async Task<bool> UpdatePlayerData(string playerID, Dictionary<string, string> dictionary, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(dictionary);
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerID, options);
            }
            catch(System.Exception)
            {
                return false;
            }
            LobbyEvents.OnLobbyUpdated(_lobby);
            return true;
        }

        

        public async Task<bool> UpdateLobbyData(Dictionary<string, string> dictionary)
        {
            Dictionary<string, DataObject> lobbyData = SerializeLobbyData(dictionary);
            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobbyData
            };
            try
            {
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            }
            catch (System.Exception)
            {
                return false;
            }
            LobbyEvents.OnLobbyUpdated(_lobby);
            return true;
        }

        public async Task<bool> RejoinLobby()
        {
            try
            {
                _lobby = await LobbyService.Instance.ReconnectToLobbyAsync(_joinedLobbiesId[0]);
                LobbyEvents.OnLobbyUpdated(_lobby);
            }
            catch(System.Exception)
            {
                return false;
            }

            _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_joinedLobbiesId[0], 1f));
            return true;
        }

        public async Task<bool> LeaveAllLobby()
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            foreach(string lobbyId in _joinedLobbiesId)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
                }
                catch(System.Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
