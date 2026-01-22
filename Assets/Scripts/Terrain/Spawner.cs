using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private ColorChanger colorChanger; // sursa RGB

    void Awake()
    {
        // Find all spawn points with the tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found!");
            return;
        }

        // Pick a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[randomIndex].transform;

        // Spawn the object
        GameObject spawnedPlayer = Instantiate(
            objectToSpawn,
            chosenSpawn.position,
            chosenSpawn.rotation
        );

        // Aplică culoarea din ColorChanger către PlayerColor
        if (colorChanger != null)
        {
            PlayerColor pc = spawnedPlayer.GetComponent<PlayerColor>();
            if (pc != null)
            {
                colorChanger.ApplyTo(pc);
            }
            else
            {
                Debug.LogWarning("PlayerColor component not found on spawned object!");
            }
        }
        else
        {
            Debug.LogWarning("ColorChanger reference not set in Spawner!");
        }
    }
}
