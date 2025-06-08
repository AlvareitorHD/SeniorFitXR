using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DanceManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridSize = 5;
    public float activationTime = 1.5f;

    public GameTimer gameTimer;            // Referencia al GameTimer desde el inspector
    public GameObject gestorPuntos;        // Objeto con el script ContadorPuntos

    private DanceTile[,] tiles;
    private DanceTile currentActiveTile;
    private DanceTile lastActiveTile;      // Última baldosa activa para evitar repeticiones
    private float timer;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        if (Application.isPlaying)
        {
            GenerateGrid();
        }
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        if (gameTimer == null || gameTimer.EstaPausado() || gameTimer.InitialDelayRestante() > 0f)
            return;

        timer += Time.deltaTime;
        if (timer > activationTime)
        {
            currentActiveTile?.Deactivate();
            ActivateRandomTile();
            timer = 0f;
        }
    }

    void GenerateGrid()
    {
#if UNITY_EDITOR
        // Limpiar baldosas previas solo en modo editor
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
#endif

        if (tilePrefab == null) return;

        Vector3 prefabSize = GetPrefabWorldSize(tilePrefab);
        float tileSizeX = prefabSize.x;
        float tileSizeZ = prefabSize.z;

        tiles = new DanceTile[gridSize, gridSize];

        float offsetX = (gridSize - 1) * tileSizeX * 0.5f;
        float offsetZ = (gridSize - 1) * tileSizeZ * 0.5f;

        Vector3 origin = transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float posX = (x * tileSizeX) - offsetX;
                float posZ = (z * tileSizeZ) - offsetZ;
                Vector3 pos = origin + new Vector3(posX, 0f, posZ);

                GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tileGO.transform.localScale = tilePrefab.transform.localScale;

                DanceTile tile = tileGO.GetComponent<DanceTile>();
                tiles[x, z] = tile;

                tile.danceManager = this; // Asignar referencia al DanceManager
            }
        }
    }

    Vector3 GetPrefabWorldSize(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.localScale = prefab.transform.localScale;

        Renderer rend = temp.GetComponentInChildren<Renderer>();
        Vector3 size = rend != null ? rend.bounds.size : Vector3.one;

#if UNITY_EDITOR
        DestroyImmediate(temp);
#else
        Destroy(temp);
#endif
        return size;
    }

    void ActivateRandomTile()
    {
        if (tiles == null) return;

        int x, z;

        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        if (lastActiveTile == null)
        {
            // Si no hay baldosa anterior, elige una aleatoria
            x = Random.Range(0, gridSize);
            z = Random.Range(0, gridSize);
        }
        else
        {
            (int currX, int currZ) = GetTilePosition(lastActiveTile);

            if (currX == -1 || currZ == -1)
            {
                // Fallback por seguridad
                x = Random.Range(0, gridSize);
                z = Random.Range(0, gridSize);
            }
            else
            {
                // Posibles desplazamientos a vecinos (8 direcciones)
                Vector2Int[] directions = new Vector2Int[]
                {
                new Vector2Int(0, 1),    // N
                new Vector2Int(1, 0),    // E
                new Vector2Int(0, -1),   // S
                new Vector2Int(-1, 0),   // O
                new Vector2Int(1, 1),    // NE
                new Vector2Int(-1, 1),   // NO
                new Vector2Int(1, -1),   // SE
                new Vector2Int(-1, -1),  // SO
                };

                foreach (var dir in directions)
                {
                    int newX = currX + dir.x;
                    int newZ = currZ + dir.y;

                    if (IsInBounds(newX, newZ) && tiles[newX, newZ] != lastActiveTile)
                    {
                        validNeighbors.Add(new Vector2Int(newX, newZ));
                    }
                }

                if (validNeighbors.Count > 0)
                {
                    Vector2Int chosen = validNeighbors[Random.Range(0, validNeighbors.Count)];
                    x = chosen.x;
                    z = chosen.y;
                }
                else
                {
                    // Si no hay vecinos válidos, elige aleatoriamente una distinta
                    do
                    {
                        x = Random.Range(0, gridSize);
                        z = Random.Range(0, gridSize);
                    } while (tiles[x, z] == lastActiveTile);
                }
            }
        }

        Color randColor = Random.ColorHSV(
            hueMin: 0f, hueMax: 1f,
            saturationMin: 0.9f, saturationMax: 1f,
            valueMin: 0.9f, valueMax: 1f
        );

        currentActiveTile = tiles[x, z];
        currentActiveTile.Activate(randColor);
    }

    // Devuelve la posición (x, z) de un tile en la matriz
    private (int, int) GetTilePosition(DanceTile tile)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (tiles[x, z] == tile)
                    return (x, z);
            }
        }
        return (-1, -1); // No encontrado
    }

    // Mezcla un array de direcciones
    private void Shuffle(Vector2Int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < gridSize && z >= 0 && z < gridSize;
    }


    public void PlayerSteppedCorrectTile(DanceTile tile)
    {
        if (tile == currentActiveTile && gameTimer != null && !gameTimer.EstaPausado())
        {
            // Reproduce sonido si hay AudioSource
            AudioSource audioSource = tile.GetComponent<AudioSource>();
            if (audioSource != null && currentActiveTile.IsActive())
                audioSource.Play();

            // Desactiva la baldosa actual para evitar múltiples activaciones
            currentActiveTile.Deactivate();
            lastActiveTile = currentActiveTile;
            currentActiveTile = null;

            // Sumar punto usando el sistema
            if (gestorPuntos != null)
            {
                ContadorPuntos contador = gestorPuntos.GetComponent<ContadorPuntos>();
                if (contador != null)
                {
                    contador.SumarPunto();
                }
                else
                {
                    Debug.LogError("No se encontró ContadorPuntos en GestorPuntos.");
                }
            }

            timer = 0f;
        }
    }
}
