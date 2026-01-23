using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float destroyDelay = 1f;

    private GameObject playerHit;
    private bool hasHit = false;

    private GameObject owner;
 
    public void SetOwner(GameObject o)
    {
        owner = o;
    }

    public GameObject GetOwner()
    {
        return owner;
    }
    public void SetEnemy(GameObject o)
    {
        playerHit = o;
    }

    public GameObject Enemy()
    {
        return playerHit;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        Debug.Log("Bullet hit: " + collision.gameObject.name);
        Invoke(nameof(DestroyBullet), destroyDelay);
        if(collision.gameObject.CompareTag("Player") == true)
            playerHit = collision.gameObject;
    }


    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
