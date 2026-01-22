using System;
using UnityEngine;

[Serializable]
public struct ClassStats
{
    public int hp;
    public int bullets;
    public float speed;
}

public class GameData : MonoBehaviour
{
    [Header("Class1 Stats")]
    public ClassStats class1 = new ClassStats
    {
        hp = 1,
        bullets = 1,
        speed = 1f
    };

    [Header("Class2 Stats")]
    public ClassStats class2 = new ClassStats
    {
        hp = 1,
        bullets = 1,
        speed = 1f
    };

    [Header("Class3 Stats")]
    public ClassStats class3 = new ClassStats
    {
        hp = 1,
        bullets = 1,
        speed = 1f
    };

    public bool TryGetStats(string className, out ClassStats stats)
    {
        stats = default;

        if (string.IsNullOrEmpty(className))
            return false;

        switch (className.ToLower())
        {
            case "class1":
                stats = class1;
                return true;

            case "class2":
                stats = class2;
                return true;

            case "class3":
                stats = class3;
                return true;

            default:
                Debug.LogWarning($"[GameData] Unknown class name: {className}");
                return false;
        }
    }
}
