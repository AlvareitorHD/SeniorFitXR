using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    public GameObject balloonPrefab;
    public int poolSize = 5;
    public float spawnRadius = 3f;
    public float spawnHeightOffset = -2f;
    public float spawnInterval = 5f;

    private GameObject[] balloons;
    private Transform player;

    void Start()
    {
        player = Camera.main.transform;

        // Crear pool de globos
        balloons = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            balloons[i] = Instantiate(balloonPrefab);
            balloons[i].SetActive(false);
        }

        // Spawnear periódicamente
        InvokeRepeating(nameof(SpawnBalloons), 1f, spawnInterval);
    }

    void SpawnBalloons()
    {
        
        int amount = Random.Range(this.poolSize / 2, this.poolSize); // Spawn entre la mitad y el total de globos en el pool

        for (int i = 0; i < amount; i++)
        {
            GameObject balloon = GetAvailableBalloon();
            if (balloon != null)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = player.position + new Vector3(randomCircle.x, spawnHeightOffset, randomCircle.y);
                balloon.transform.position = spawnPos;
                balloon.SetActive(true);
            }
        }
    }

    GameObject GetAvailableBalloon()
    {
        foreach (var b in balloons)
        {
            if (!b.activeInHierarchy)
                return b;
        }
        return null;
    }
}
