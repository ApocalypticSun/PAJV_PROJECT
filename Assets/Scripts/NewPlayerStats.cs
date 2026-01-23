using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class NewPlayerStats : MonoBehaviour
{
    [Header("Scene Lookup (names/paths)")]
    [SerializeField] private string playerManagerName = "Player Manager";
    [SerializeField] private string uiManagerName = "UI Manager";
    [SerializeField] private string hpValuePathUnderUIManager = "Canvas/HP Value";
    [SerializeField] private string ammoValuePathUnderUIManager = "Canvas/Ammo Value";

    [Header("Class Name")]
    [SerializeField] private string className = "Class1";

    [Header("Runtime Stats (read-only in inspector)")]
    [SerializeField] private int hp;
    [SerializeField] private int bullets;
    [SerializeField] private float speed;
    public int Kills = 0;


    public int HP => hp;
    public int Bullets => bullets;
    public float Speed => speed;

    private GameData gameData;
    private TMP_Text healthText;
    private TMP_Text ammoText;

    private void Awake()
    {
        AutoWire();
        className = PlayFabController.Instance.clasa;
    }

    private void Start()
    {
        ApplyClassFromString();
        UpdateUI();
    }

    private void AutoWire()
    {
        // --- GameData din Player Manager ---
        GameObject pm = GameObject.Find(playerManagerName);
        if (pm != null)
        {
            gameData = pm.GetComponent<GameData>();
        }

        if (gameData == null)
            Debug.LogError("PlayerStats: GameData not found on Player Manager.", this);

        // --- UI texts din UI Manager ---
        GameObject ui = GameObject.Find(uiManagerName);
        if (ui != null)
        {
            Transform hpT = ui.transform.Find(hpValuePathUnderUIManager);
            if (hpT != null) healthText = hpT.GetComponent<TMP_Text>();

            Transform ammoT = ui.transform.Find(ammoValuePathUnderUIManager);
            if (ammoT != null) ammoText = ammoT.GetComponent<TMP_Text>();
        }

        if (healthText == null)
            Debug.LogWarning("PlayerStats: HP Value TMP not found (UI path wrong?).", this);

        if (ammoText == null)
            Debug.LogWarning("PlayerStats: Ammo Value TMP not found (UI path wrong?).", this);
    }

    public void SetClass(string newClassName)
    {
        className = newClassName;
        ApplyClassFromString();
        UpdateUI();
    }

    private void ApplyClassFromString()
    {
        if (gameData == null) return;

        if (gameData.TryGetStats(className, out ClassStats stats))
        {
            hp = stats.hp;
            bullets = stats.bullets;
            speed = stats.speed;

            Debug.Log($"{name} applied class '{className}' => HP={hp}, Bullets={bullets}, Speed={speed}");
        }
        else
        {
            Debug.LogWarning($"{name} could not apply class '{className}', no stats found.", this);
        }
    }

    public void ConsumeBullet(int amount = 1)
    {
        bullets = Mathf.Max(0, bullets - amount);
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        UpdateUI();
    }
    

    private void UpdateUI()
    {
        if (healthText != null) healthText.text = hp.ToString();
        if (ammoText != null) ammoText.text = bullets.ToString();
    }
}
