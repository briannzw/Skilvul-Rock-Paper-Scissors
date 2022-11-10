using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class GameServiceInitialization : MonoBehaviour
{
    public string enviromentName;

    async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Uninitialized) return;

        InitializationOptions options = new InitializationOptions();
        options.SetEnvironmentName(enviromentName);
        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)  
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
