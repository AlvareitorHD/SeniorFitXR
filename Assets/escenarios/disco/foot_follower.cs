using UnityEngine;

public class FootFollower : MonoBehaviour
{
    public Transform playerRig;         // Asigna aquí el rig principal (por ejemplo, OVRCameraRig o PlayerController)
    public float forwardOffset = 0f;  // Qué tan adelante del rig se sitúa
    public float height = 0.0f;         // Altura (Y) fija respecto al suelo, puede ser 0 si está en el suelo

    void Update()
    {
        if (playerRig == null) return;

        Vector3 rigPosition = playerRig.position;
        Vector3 rigForward = new Vector3(playerRig.forward.x, 0, playerRig.forward.z).normalized;

        Vector3 newPosition = rigPosition + rigForward * forwardOffset;
        newPosition.y = height; // Mantenerlo al nivel deseado

        transform.position = newPosition;
    }
}
