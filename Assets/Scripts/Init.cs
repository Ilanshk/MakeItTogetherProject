using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Services.Vivox;

namespace Game
{
    public class Init : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            
            await UnityServices.InitializeAsync();
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                AuthenticationService.Instance.SignedIn += OnSignedIn;
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                await VivoxService.Instance.InitializeAsync();
                if (Application.HasUserAuthorization(UserAuthorization.Microphone))
                    await VivoxService.Instance.LoginAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    string username = PlayerPrefs.GetString("Username");
                    if (username == "")
                    {
                        username = "Player";
                        PlayerPrefs.SetString("Username", username);
                    }

                    SceneManager.LoadSceneAsync("MainMenu");
                }
            }
        }

        private void OnSignedIn()
        {
            Debug.Log($"Token: {AuthenticationService.Instance.AccessToken}");
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

