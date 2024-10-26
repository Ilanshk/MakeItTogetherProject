using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;

public class VivoxPlayer : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    private async void OnUserLoggedIn()
    {
        
        bool micIsAllowed = Application.HasUserAuthorization(UserAuthorization.Microphone);
        if(micIsAllowed)
        {
            await VivoxService.Instance.LoginAsync();
        }
        else
        {
            
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
