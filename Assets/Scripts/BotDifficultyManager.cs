using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

struct UserAttributes { }
struct AppAttributes { }

public class BotDifficultyManager : MonoBehaviour
{
    public Bot bot;
    public int selectedDifficulty;
    public Stats[] botDifficulties;

    [Header("Remote Config Parameters")]
    public bool enableRemoteConfig = false;
    public string difficultyKey = "Difficulty";

    IEnumerator Start()
    {
        bot.SetStats(botDifficulties[selectedDifficulty]);

        if (!enableRemoteConfig) yield break;

        yield return new WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn);

        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;
        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
    }

    private void OnRemoteConfigFetched(ConfigResponse response)
    {
        if (!RemoteConfigService.Instance.appConfig.HasKey(difficultyKey)) return;

        switch (response.requestOrigin)
        {
            case ConfigOrigin.Default:
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                selectedDifficulty = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                selectedDifficulty = Mathf.Clamp(selectedDifficulty, 0, botDifficulties.Length - 1);
                
                bot.SetStats(botDifficulties[selectedDifficulty]);
                break;
        }
    }
}
