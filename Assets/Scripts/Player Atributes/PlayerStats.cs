using UnityEngine;
using TMPro;
using System;
using NaughtyAttributes;

public class PlayerStats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameData gameData;

    [Header("Class Name (String)")]
    [SerializeField] private string className = "Class1";

    [Header("Runtime Stats")]
    [SerializeField] private int hp;
    [SerializeField] private int bullets;
    [SerializeField] private float speed;
    [SerializeField] TMP_Text Health;
    [SerializeField] TMP_Text Ammo;

    private int Healthpoints;
    public int HP => hp;
    public int Bullets => bullets;
    public float Speed => speed;

    private void Start()
    {
        ApplyClassFromString();
    }

    public void SetClass(string newClassName)
    {
        className = newClassName;
        ApplyClassFromString();
    }

    private void ApplyClassFromString()
    {
        if (gameData == null)
        {
            Debug.LogError("GameData not assigned on PlayerStats", this);
            return;
        }

        if (gameData.TryGetStats(className, out ClassStats stats))
        {
            hp = stats.hp;
            bullets = stats.bullets;
            speed = stats.speed;

            Debug.Log($"{name} applied class '{className}' => HP={hp}, Bullets={bullets}, Speed={speed}");
        }
        else
        {
            Debug.LogWarning($"{name} could not apply class '{className}', no stats found.");
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(bullets > 0)
            {
                //Sa traga
            }
            else
            {
                Debug.Log("Gloante pula");
            }
            Ammo.text = bullets.ToString();
        }
        if(Healthpoints==0) //Aici Punem daca isi ia hit
        {
            //Ceva constanta de dmg scazuta din Hp => Hp nou si se updateaza TMP
            //DMG sa vina din bullet, trebuie adaugat la fiecare clasa 
        }
        else
        {
            //Verificare de debugging
        }
        //If atunci cand atinge 0 hp-ul sa se distruga obiectul player-ului (Ca sa nu dea crash pur si simplu il face un ghost si ii blocam scripturile)
    }
}
