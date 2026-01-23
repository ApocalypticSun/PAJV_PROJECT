using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Button myButton;
    public Button myButton2;

    public TMP_Text displayName;


    void Start()
    {
        //myButton.onClick.AddListener(() => PlayFabController.Instance.StartCloudUpdatePlayerStats());
        //myButton2.onClick.AddListener(() => PlayFabController.Instance.GetLeaderboard());

        PlayFabController.Instance.OnDisplayNameLoaded += SetDisplayName;

        // Optional: handle case where name already loaded
        if (!string.IsNullOrEmpty(PlayFabController.Instance.displayName))
        {
            SetDisplayName(PlayFabController.Instance.displayName);
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
}
