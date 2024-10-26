using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeacherCanvaManager : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    private void OnEnable()
    {
        _exitButton.onClick.AddListener(ExitFunction);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveAllListeners();
    }

    private void ExitFunction()
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            VivoxService.Instance.LeaveAllChannelsAsync();
        }
        AuthenticationService.Instance.SignOut();
        VivoxService.Instance.LogoutAsync();
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
