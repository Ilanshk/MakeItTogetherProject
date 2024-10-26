using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;
using UnityEngine.UI;
using Game.Data;
using System;
using Game.Events;

namespace Game
{
    
    public class LobbyUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Image _mapImage;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private TextMeshProUGUI _mapName;
        [SerializeField] private MapSelectionData _mapSelectionData;

        private int _currentMapIndex = 0;

        private void OnEnable()
        {
            if(GameLobbyManager.Instance.IsHost)
            {
                _leftButton.onClick.AddListener(OnLeftButtonClicked);
                _rightButton.onClick.AddListener(OnRightButtonClicked);
                _startButton.onClick.AddListener(OnStartButtonClicked);
                LobbyEvents.OnLobbyReady += OnLobbyReady;
            }
            _readyButton.onClick.AddListener(OnReadyPressed);
            

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
        }

        

        private void OnDisable()
        {
            _leftButton.onClick.RemoveAllListeners();
            _rightButton.onClick.RemoveAllListeners();
            _readyButton.onClick.RemoveAllListeners();
            _startButton.onClick.RemoveAllListeners();
            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyEvents.OnLobbyReady -= OnLobbyReady;

        }

        // Start is called before the first frame update
        async void Start()
        {
            lobbyCodeText.text = $"Lobby code: {GameLobbyManager.Instance.GetLobbyCode()}";
            if(!GameLobbyManager.Instance.IsHost)
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false);
                lobbyCodeText.text = "";
            }
            else
            {
                await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
            }
        }

        private async void OnReadyPressed()
        {
            bool succeed = await GameLobbyManager.Instance.SetPlayerReady();
            if(succeed)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }
        private async void OnLeftButtonClicked()
        {
            if (_currentMapIndex - 1 >= 0)
            {
                _currentMapIndex--;
            }
            else
                _currentMapIndex = _mapSelectionData.Maps.Count - 1;

            UpdateMap();
            bool succeeded = await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        

        private async void OnRightButtonClicked()
        {
            if (_currentMapIndex + 1 <= _mapSelectionData.Maps.Count - 1)
            {
                _currentMapIndex++;
            }
            else
                _currentMapIndex = 0;
            UpdateMap();
            bool succeeded = await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        private void UpdateMap()
        {
            _mapImage.GetComponent<Image>().color = _mapSelectionData.Maps[_currentMapIndex].MapThumbnail;
            _mapName.text = _mapSelectionData.Maps[_currentMapIndex].MapName;
        }

        private void OnLobbyUpdated()
        {
            _currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            UpdateMap();
        }

        private void OnLobbyReady()
        {
            _startButton.gameObject.SetActive(true);
        }

        private async void OnStartButtonClicked()
        {
            await GameLobbyManager.Instance.StartGame();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


