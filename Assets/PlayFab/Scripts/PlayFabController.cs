using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayFabController : MonoBehaviour
{

    public static PlayFabController Instance;
    public Action<string> OnDisplayNameLoaded;
    public Action OnLeaderboardUpdated;

    public string displayName;

    private string myID;

    //D57EBD0D4BFB4FC2
    //723434C628E9B976

    public string customId;

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

    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // args[0] is usually the path to the exe, so the first parameter is args[1]
        if (args.Length > 1)
        {
            customId = args[1];
            Debug.Log("Received customId: " + customId);
        }
        else
        {
            Debug.Log("No customId received from launcher.");
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
            CustomId = customId,
            CreateAccount = false
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFail);

      


    }

    void OnApplicationQuit()
    {
        StartCloudUpdatePlayerStats();

       
    }


    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success");

        myID = result.PlayFabId;

        GetDisplayName();
        StartCloudUpdatePlayerStats();
        GetStats();

        GetLeaderboard();

        GetPlayerData();



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

    ///----------------------------------------STATISTICS----------------------------------------///


    public int kills;

    public int totalKills;

    public void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport()));
    }


    void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Recieved staistics");
        foreach (var eachStat in result.Statistics)
        {
            switch (eachStat.StatisticName)
            {
                case "Kills":
                    totalKills = eachStat.Value; break;
            }

        }

    }

    public void StartCloudUpdatePlayerStats()
    {
        Debug.Log($"StartCloudUpdatePlayerStats");
        Debug.Log("Logged in: " + PlayFabClientAPI.IsClientLoggedIn());
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats",
            FunctionParameter = new { p_kills = kills, },
            GeneratePlayStreamEvent = true
        }, OnCloudUpdatePlayerStats, OnErrorShared);
    }


    private void OnCloudUpdatePlayerStats(ExecuteCloudScriptResult result)
    {
        Debug.Log("OnCloudUpdatePlayerStats");
        if (result.Error != null)
        {
            Debug.LogError(result.Error.Message);
            return;
        }

        JsonObject jsonResult = (JsonObject)result.FunctionResult;

        int newTotal = Convert.ToInt32(jsonResult["newKills"]);
        totalKills = newTotal;
        Debug.Log("New Total Kills: " + newTotal);
    }


    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }


    ///----------------------------------------LEADERBOARD----------------------------------------///

    public List<PlayerLeaderboardEntry> players;
    public void GetLeaderboard()
    {
        var requestLeaderoard = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "Kills",
            MaxResultsCount = 5,

        };
        PlayFabClientAPI.GetLeaderboard(requestLeaderoard, OnGetLeaderboard, OnErrorLeaderboard);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            Debug.Log(player.DisplayName + ": " + player.StatValue);
            players.Add(player);
        }
        OnLeaderboardUpdated?.Invoke();

    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.Log($"Error {error.GenerateErrorReport()}");
    }



    ///----------------------------------------PLAYER_DATA----------------------------------------///

    public string clasa;
    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myID,
            Keys = null

        }, UserDataSuccess, UserDataFail);
    }

    void UserDataSuccess(GetUserDataResult result)
    {
        if (result == null || !result.Data.ContainsKey("Class"))
        {
            Debug.Log("no class");
        }
        else
        {
            clasa = result.Data["Class"].Value;
        }
    }


    void UserDataFail(PlayFabError error)
    {

    }


    public void SetUserData(string p_clasa)
    {
        Debug.Log("class set data");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Class", p_clasa}
            }
        }, SetDataSuccess, SetDataFail);
    }

    void SetDataSuccess(UpdateUserDataResult result)
    {

    }


    void SetDataFail(PlayFabError error)
    {

    }

}
