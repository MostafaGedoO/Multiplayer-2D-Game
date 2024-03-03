using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
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
         
        authenticationState = AuthenticationState.Authenticating;
        int _tries = 0;

        while(authenticationState == AuthenticationState.Authenticating & _tries < _MaxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                authenticationState = AuthenticationState.Authenticated;
                break;
            }
            
            _tries++;
            await Task.Delay(1000);
        }

        return authenticationState;
    }
}


public enum AuthenticationState { NotAuthenticated, Authenticating, Authenticated, Error, TimeOut }