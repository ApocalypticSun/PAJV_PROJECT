using UnityEngine;
using System.Collections;
using TMPro;
public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    public ParticleSystem flash;
    public GameObject impact;
    public int ammo = 40;
    public TMP_Text Ammunition;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(ammo > 0)
            {
                Shoot();
            }
            else
            {
                Debug.Log("Gloante pula");
            }
            Ammunition.text = "Ammo: " + ammo.ToString();
        }
    }
    void Shoot()
    {
        flash.Play();
        ammo -= 1;
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward,out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
            //EnemyAi enemy = hit.transform.GetComponent<EnemyAi>();
            /* if(enemy != null)
            {
                enemy.TakeDamage(damage*2);
            } */
        }
        GameObject impactGO = Instantiate(impact,hit.point,Quaternion.LookRotation(hit.normal));
        Destroy(impactGO,1f);
    }
    public void AddAmmo(int amount)
    {
        ammo += amount;
        Debug.Log("Catchu");
    }
}
