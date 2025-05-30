using UnityEngine;

public class LockXZPosition : MonoBehaviour
{
    private Vector3 fixedPosition;

    void Start()
    {
        fixedPosition = transform.position;
    }

    void Update()
    {
        // Permite movimiento en Y (para mantener altura por si el jugador se agacha)
        transform.position = new Vector3(fixedPosition.x, transform.position.y, fixedPosition.z);
    }
}
