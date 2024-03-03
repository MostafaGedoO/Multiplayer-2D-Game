using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager 
{
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        AuthenticationState _authenticationState = await AuthenticationHandler.DoAuthentication();

        if (_authenticationState == AuthenticationState.Authenticated)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
