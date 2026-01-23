using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayFabController : MonoBehaviour
{

    public static PlayFabController Instance;
    public Action<string> OnDisplayNameLoaded;

    public string displayName;

    //D57EBD0D4BFB4FC2

    private void OnEnable()
    {
        if (PlayFabController.Instance == null)
        {
            PlayFabController.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (PlayFabController.Instance != this)
            {
                Destroy(this.gameObject);
            }
        }

    }

    void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "E14D9";
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = "D57EBD0D4BFB4FC2",
            CreateAccount = false
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFail);
        
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success");

        GetDisplayName();

    }

    private void OnLoginFail(PlayFabError error)
    {
        Debug.Log("Login failed");
        Debug.Log(error.GenerateErrorReport());

    }

    void GetDisplayName()
    {
        var request = new PlayFab.ClientModels.GetPlayerProfileRequest
        {
            PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            ProfileConstraints = new PlayFab.ClientModels.PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnProfileSuccess, OnProfileError);
    }

    private void OnProfileSuccess(PlayFab.ClientModels.GetPlayerProfileResult result)
    {
        if (result.PlayerProfile != null)
        {
            displayName = result.PlayerProfile.DisplayName;
            Debug.Log("Display Name: " + displayName);
            OnDisplayNameLoaded?.Invoke(displayName);
        }
        else
        {
            Debug.Log("Profile not found");
        }
    }

    private void OnProfileError(PlayFabError error)
    {
        Debug.Log("Profile error");
        Debug.Log(error.GenerateErrorReport());
    }


    public string GiveDisplayName()
    {
        return displayName;
    }

}
