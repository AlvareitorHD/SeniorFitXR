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

    // Método OnEnable se llama cuando el script se activa
    void OnEnable()
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
        // Spawn de globos randoms alrededor del jugador
        int amount = Random.Range(poolSize/3, poolSize + 1); // Número aleatorio de globos a spawnear

        for (int i = 0; i < amount; i++)
        {
            GameObject balloon = GetAvailableBalloon();
            if (balloon != null)
            {
                // Este vector2 genera un punto aleatorio dentro de un círculo de radio spawnRadius
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

                // La posición de spawn es la posición del jugador más un desplazamiento aleatorio en el plano XZ
                // pero hay que evitar que el globo aparezca sobre el propio jugador
                // Aseguramos que el globo no aparezca demasiado cerca del jugador
                while (randomCircle.magnitude < 1f)
                {
                    randomCircle = Random.insideUnitCircle * spawnRadius;
                }
                Vector3 spawnPos = player.position + new Vector3(randomCircle.x, spawnHeightOffset, randomCircle.y);
                balloon.transform.position = spawnPos;
                balloon.SetActive(true);
            }
        }
    }

    public void OnDisable()
    {
        // Desactivar todos los globos al desactivar el script
        foreach (var b in balloons)
        {
            b.SetActive(false);
        }
        CancelInvoke(nameof(SpawnBalloons)); // Detener la invocación periódica
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
