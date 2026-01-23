
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Button myButton;
    public Button myButton2;

    public TMP_Text displayName;

    public GameObject leaderboard;
    public GameObject mainManu;
    public GameObject customization;


    public TMP_Text p1;
    public TMP_Text p2;
    public TMP_Text p3;
    public TMP_Text p4;
    public TMP_Text p5;



    void Start()
    {
        myButton.onClick.AddListener(() => PlayFabController.Instance.GetPlayerData());
        myButton2.onClick.AddListener(() => PlayFabController.Instance.GetLeaderboard());

        PlayFabController.Instance.OnDisplayNameLoaded += SetDisplayName;
        PlayFabController.Instance.OnLeaderboardUpdated += SetLeaderboard;

        if (!string.IsNullOrEmpty(PlayFabController.Instance.displayName))
        {
            SetDisplayName(PlayFabController.Instance.displayName);
        }

      
    }

    public void SetLeaderboard()
    {
     

        TMP_Text[] textFields = new TMP_Text[] { p1, p2, p3, p4, p5 };

        for (int i = 0; i < PlayFabController.Instance.players.Count && i < textFields.Length; i++)
        {
         
            if (PlayFabController.Instance.players[i] != null)
            {
              
                textFields[i].text = (i + 1) + ". " + PlayFabController.Instance.players[i].DisplayName + " [ " + PlayFabController.Instance.players[i].StatValue + " ]";
            }
            else
            {
                Debug.Log("no PLAYERRS");
            }
        }
    }

    void SetDisplayName(string name)
    {
        displayName.text = "WELCOME " + name;
        Debug.Log("from panel: " + name);
    }

    private void OnDestroy()
    {
        if (PlayFabController.Instance != null)
            PlayFabController.Instance.OnDisplayNameLoaded -= SetDisplayName;
    }


    public void LoadLeaderBoard()
    {
        leaderboard.SetActive(true);
        mainManu.SetActive(false);
    }

    public void LoadCustomization()
    {
        customization.SetActive(true);
        mainManu.SetActive(false);
    }

    public void LoadMainManu()
    {
        mainManu.SetActive(true);
        customization.SetActive(false);
        leaderboard.SetActive(false);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("ShootingRange");
    }

    public void SetToCLASS1()
    {
        PlayFabController.Instance.clasa = "Class1";
        PlayFabController.Instance.SetUserData(PlayFabController.Instance.clasa);
    }

    public void SetToCLASS2()
    {
        PlayFabController.Instance.clasa = "Class2";
        PlayFabController.Instance.SetUserData(PlayFabController.Instance.clasa);

    }

    public void SetToCLASS3()
    {
        PlayFabController.Instance.clasa = "Class3";
        PlayFabController.Instance.SetUserData(PlayFabController.Instance.clasa);
    }


}
