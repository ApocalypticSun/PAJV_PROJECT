using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewPlayerStats playerStats;
    [SerializeField] private int HP;

    [Header("Damage")]
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float serverInitDelay = 2f;

    [Header("Ghost Mode (disable gameplay)")]
    [SerializeField] private Behaviour[] behavioursToDisable;
    [SerializeField] private Collider[] collidersToDisable;
    [SerializeField] private Rigidbody[] rigidbodiesToFreeze;

    [Header("Invisible Mode (visuals)")]
    [SerializeField] private Renderer[] renderersToHide;

    private bool isReady = false;
    private bool isDead = false;

    private void Start()
    {
        if (playerStats == null)
            playerStats = GetComponent<NewPlayerStats>();

        StartCoroutine(InitAfterServerDelay());
    }

    private IEnumerator InitAfterServerDelay()
    {
        yield return new WaitForSeconds(serverInitDelay);

        if (playerStats == null)
        {
            Debug.LogError("PlayerHP: NewPlayerStats not found.");
            yield break;
        }

        HP = playerStats.HP;
        isReady = true;

        CheckDeathAndGhost();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isReady || isDead) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            // ignore own bullet
            Bullet b = collision.gameObject.GetComponent<Bullet>();
            if (b != null && b.GetOwner() == transform.root.gameObject)
                return;

            TakeDamage(bulletDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady || isDead) return;

        if (other.CompareTag("Bullet"))
        {
            // ignore own bullet
            Bullet b = other.GetComponent<Bullet>();
            if (b != null && b.GetOwner() == transform.root.gameObject)
                return;

            TakeDamage(bulletDamage);
        }
    }

    private void TakeDamage(int dmg)
    {
        HP -= dmg;
        Debug.Log($"PlayerHP: {HP}");
        CheckDeathAndGhost();
    }

    private void CheckDeathAndGhost()
    {
        if (!isDead && HP <= 0)
            EnterGhostMode();
    }

    private void EnterGhostMode()
    {
        isDead = true;
        Debug.Log("Ghost + Invisible mode ON");

        // disable gameplay scripts
        if (behavioursToDisable != null)
        {
            foreach (var b in behavioursToDisable)
                if (b != null) b.enabled = false;
        }

        // disable colliders
        if (collidersToDisable != null)
        {
            foreach (var c in collidersToDisable)
                if (c != null) c.enabled = false;
        }

        // freeze rigidbodies
        if (rigidbodiesToFreeze != null)
        {
            foreach (var rb in rigidbodiesToFreeze)
            {
                if (rb == null) continue;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // hide renderers
        if (renderersToHide != null)
        {
            foreach (var r in renderersToHide)
                if (r != null) r.enabled = false;
        }
    }

    // optional: respawn / revive
    public void Revive()
    {
        isDead = false;

        if (behavioursToDisable != null)
        {
            foreach (var b in behavioursToDisable)
                if (b != null) b.enabled = true;
        }

        if (collidersToDisable != null)
        {
            foreach (var c in collidersToDisable)
                if (c != null) c.enabled = true;
        }

        if (rigidbodiesToFreeze != null)
        {
            foreach (var rb in rigidbodiesToFreeze)
                if (rb != null) rb.isKinematic = false;
        }

        if (renderersToHide != null)
        {
            foreach (var r in renderersToHide)
                if (r != null) r.enabled = true;
        }

        // reset HP from stats (optional but usually desired on revive)
        if (playerStats != null)
            HP = playerStats.HP;

        CheckDeathAndGhost();
    }
}
