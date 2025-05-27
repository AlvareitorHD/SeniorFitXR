using UnityEngine;

public class ForzarPosicionYAltura : MonoBehaviour
{
    public OVRCameraRig cameraRig; // Asigna tu OVRCameraRig en el Inspector  
    public float alturaFija = 1.7f; // Altura simulada (en metros)  

    void Start()
    {
        // Posicionar el OVRCameraRig en el origen del mundo  
        cameraRig.transform.position = Vector3.zero;
        // Posicionar un poco mas hacia atras
        cameraRig.transform.position += new Vector3(0, 0, -0.5f);
        cameraRig.transform.rotation = Quaternion.identity;

        // Mover la cámara (centro de ojos) a una altura simulada  
        if (cameraRig.centerEyeAnchor != null)
        {
            Vector3 localPos = cameraRig.centerEyeAnchor.localPosition;
            localPos.y = alturaFija;
            cameraRig.centerEyeAnchor.localPosition = localPos;
        }
    }
}
