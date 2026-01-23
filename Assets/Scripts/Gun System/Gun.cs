using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform barrel;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private NewPlayerStats playerStats;

    [Header("Timing")]
    [SerializeField] private float serverInitDelay = 2f; // timp de așteptare server
    [SerializeField] private float reloadTime = 10f;

    private float nextFireTime = 0f;
    private bool isReloading = false;
    private bool isReady = false;

    private int currentBullets;
    private float fireDelay;

    private PlayerHP playerHP;

    private void Start()
    {
        if (playerStats == null)
            playerStats = GetComponentInParent<NewPlayerStats>();

        // IMPORTANT: PlayerHP trebuie să fie pe player (părinte/root)
        playerHP = GetComponentInParent<PlayerHP>();

        StartCoroutine(WaitForServerData());
    }

    private IEnumerator WaitForServerData()
    {
        yield return new WaitForSeconds(serverInitDelay);

        if (playerStats == null)
        {
            Debug.LogError("Gun: NewPlayerStats not found.");
            yield break;
        }

        currentBullets = playerStats.Bullets;
        fireDelay = 1f / Mathf.Max(playerStats.Speed, 0.1f);

        isReady = true;
        Debug.Log("Gun ready to fire!");
    }

    private void Update()
    {
        if (!isReady || isReloading)
            return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentBullets > 0)
            {
                Shoot();
                currentBullets--;
                nextFireTime = Time.time + fireDelay;

                if (currentBullets <= 0)
                    StartReload();
            }
        }
    }

        private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, barrel.position, barrel.rotation);

        // SET OWNER
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
            b.SetOwner(transform.root.gameObject);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = barrel.forward * 32f;
    }



    private void StartReload()
    {
        isReloading = true;
        Debug.Log("Out of ammo... Reloading");

        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        currentBullets = playerStats.Bullets;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}
