using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _joinScreen;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _rejoinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _submitCodeButton;
        [SerializeField] private TextMeshProUGUI _codeText;

        // Start is called before the first frame update
        void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _submitCodeButton.onClick.AddListener(OnSubmitCodeClicked);
            _leaveButton.onClick.AddListener(OnLeaveGameClicked);
        }

        void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _submitCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);
            _leaveButton.onClick.RemoveListener(OnLeaveGameClicked);
        }



        private async void Start()
        {
            if(await GameLobbyManager.Instance.HasActiveLobbies())
            {
                _hostButton.gameObject.SetActive(false);
                _joinButton.gameObject.SetActive(false);
                _rejoinButton.gameObject.SetActive(true);
                _leaveButton.gameObject.SetActive(true);
                _rejoinButton.onClick.AddListener(OnRejoinGameClicked);
                _rejoinButton.onClick.AddListener(OnLeaveGameClicked);
            }
        }

        private async void OnLeaveGameClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.LeaveAllLobby();
            if (succeeded)
            {
                Debug.Log("All lobbies left");
                _hostButton.gameObject.SetActive(true);
                _joinButton.gameObject.SetActive(true);
                _rejoinButton.gameObject.SetActive(false);
                _leaveButton.gameObject.SetActive(false);
            }
        }

        private async void OnRejoinGameClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.RejoinGame();
            if(succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private async void OnHostClicked()
        {
            bool succeeded = await GameLobbyManager.Instance.CreateLobby();
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }

        }

        private void OnJoinClicked()
        {
            _mainMenu.SetActive(false);
            _joinScreen.SetActive(true);
        }

        private async void OnSubmitCodeClicked()
        {
            string code = _codeText.text;
            code = code.Substring(0, code.Length - 1);
            bool succeeded = await GameLobbyManager.Instance.JoinLobby(code);
            if (succeeded)
            {
                SceneManager.LoadSceneAsync("Lobby");
            }
        }
    }
}


