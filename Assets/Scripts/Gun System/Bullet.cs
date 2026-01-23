using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float destroyDelay = 1f;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        Debug.Log("Bullet hit: " + collision.gameObject.name);
        Invoke(nameof(DestroyBullet), destroyDelay);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
