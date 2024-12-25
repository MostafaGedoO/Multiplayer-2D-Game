using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler 
{
    public static AuthenticationState authenticationState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuthentication(int _MaxTries = 5)
    {
        if(authenticationState == AuthenticationState.Authenticated)
        {
            return authenticationState;
        }
     
        if(authenticationState == AuthenticationState.Authenticating)
        {
            Debug.Log("Aleardy Authenticating!");
            await Authenticating();
            return authenticationState;
        }

        await SignInAnonymouslyAsync(_MaxTries);

        return authenticationState;
    }

    private static async Task<AuthenticationState> Authenticating()
    {
        if(authenticationState == AuthenticationState.Authenticating | authenticationState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return authenticationState;
    }

    private static async Task SignInAnonymouslyAsync(int _MaxTries)
    {
        authenticationState = AuthenticationState.Authenticating;
        int _tries = 0;

        while (authenticationState == AuthenticationState.Authenticating & _tries < _MaxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authenticationState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException ex)
            { 
                Debug.LogException(ex);
                authenticationState = AuthenticationState.Error;
            }
            catch(RequestFailedException ex)
            {
                Debug.LogException(ex);
                authenticationState = AuthenticationState.Error;
            }

            _tries++;
            await Task.Delay(1000);
        }

        if(authenticationState != AuthenticationState.Authenticated)
        {
            Debug.LogError($"Player is not authenticated after {_MaxTries} tries");
            authenticationState = AuthenticationState.TimeOut;
        }
    }
}


public enum AuthenticationState { NotAuthenticated, Authenticating, Authenticated, Error, TimeOut }