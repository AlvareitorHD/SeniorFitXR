using UnityEngine;
using UnityEngine.SceneManagement;

public class BalloonBehaviour : MonoBehaviour
{
    [Header("Movimiento")]
    public float riseSpeed = 0.5f;
    public float swayAmplitude = 0.3f;
    public float swayFrequency = 1f;
    public float lifeTime = 5f;

    [Header("Mirada y explosi�n")]
    public float gazeTimeToPop = 2f;
    public Renderer balloonRenderer;
    // Color de inicio (rosilla) y fin para el globo
    public Color startColor = new Color(1f, 0.5f, 0.5f); // Rosado claro
    public Color endColor = Color.red;
    public AudioSource audioSource;
    // Camara del jugador (CenterEyeAnchor del CameraRig de Meta)
    Transform playerCamera;

    [Header("Escalado al mirar")]
    public Vector3 normalScale = new Vector3(20, 20, 20);
    public Vector3 lookedAtScale = new Vector3(30, 30, 30);

    [Header("Raycast Ajuste")]
    public float verticalRaycastOffset = 0.5f;

    [Header("UI")]
    [SerializeField] private GameObject lookCrosshairCanvas; // Asignar en el inspector

    public ContadorPuntos scoreManager;

    private float gazeTimer = 0f;
    private float lifeTimer = 0f;
    private float swayOffset;
    private Material balloonMaterial;
    private bool poppedByPlayer = false;

    private void Start()
    {
        GameObject centerEye = GameObject.Find("CenterEyeAnchor");
        if (centerEye != null)
        {
            playerCamera = centerEye.transform;
        }
        else
        {
            Debug.LogError("No se encontr� el objeto CenterEyeAnchor en la escena.");
        }
    }

    void OnEnable()
    {
        if (lookCrosshairCanvas != null)
            lookCrosshairCanvas.SetActive(false);

        balloonRenderer.enabled = true;
        ReiniciarExplosion();

        if (scoreManager == null)
        {
            scoreManager = Object.FindFirstObjectByType<ContadorPuntos>();
            if (scoreManager == null)
                Debug.LogError("No se encontr� un script ContadorPuntos en la escena.");
        }

        gazeTimer = 0f;
        lifeTimer = 0f;
        poppedByPlayer = false;
        swayOffset = Random.Range(0f, 2f * Mathf.PI);

        if (balloonRenderer != null)
        {
            Material[] mats = balloonRenderer.materials;
            if (mats.Length > 1)
            {
                balloonMaterial = mats[1];
                balloonMaterial.color = startColor;
                mats[1] = balloonMaterial;
                balloonRenderer.materials = mats;
            }
            else
            {
                Debug.LogWarning("El renderer no tiene suficientes materiales para acceder al segundo.");
            }
        }
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swayFrequency + swayOffset) * swayAmplitude;
        transform.position += new Vector3(sway * Time.deltaTime, riseSpeed * Time.deltaTime, 0);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            poppedByPlayer = false;
            Pop();
            return;
        }

        Vector3 hitPoint, hitNormal;
        if (IsUserLookingAtBalloon(out hitPoint, out hitNormal))
        {
            gazeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(gazeTimer / gazeTimeToPop);

            if (balloonMaterial != null)
                balloonMaterial.color = Color.Lerp(startColor, endColor, progress);

            //transform.localScale = Vector3.Lerp(normalScale, lookedAtScale, progress);

            if (lookCrosshairCanvas != null)
            {
                lookCrosshairCanvas.SetActive(true);
                // Actualiza la posici�n y rotaci�n del canvas de mira
                lookCrosshairCanvas.transform.position = hitPoint + hitNormal * 0.1f; // Ajusta la posici�n para que no est� dentro del globo

                // Quaternion.LookRotation(hitNormal);
                //lookCrosshairCanvas.transform.rotation = Quaternion.LookRotation(hitNormal, Vector3.up);

                // Alternativa si prefieres que mire hacia la c�mara:
                lookCrosshairCanvas.transform.LookAt(Camera.main.transform);
                lookCrosshairCanvas.transform.Rotate(0, 180f, 0);
            }

            if (gazeTimer >= gazeTimeToPop)
            {
                // Si el jugador ha mirado el globo el tiempo suficiente, lo explota
                // Desactiva el canvas de mira
                if (lookCrosshairCanvas != null)
                    lookCrosshairCanvas.SetActive(false);
                poppedByPlayer = true;
                Pop();
            }
        }
        else
        {
            gazeTimer = 0f;
            if (balloonMaterial != null)
                balloonMaterial.color = startColor;

            if (lookCrosshairCanvas != null)
                lookCrosshairCanvas.SetActive(false);

            //transform.localScale = Vector3.Lerp(transform.localScale, normalScale, Time.deltaTime * 5f);
        }
    }

    // out es una palabra clave que permite devolver m�ltiples valores desde un m�todo
    bool IsUserLookingAtBalloon(out Vector3 hitPoint, out Vector3 hitNormal)
    {
        // La camara es el objeto centeranchor del camera rig de Meta

        Vector3 rayOrigin = playerCamera.position;  //Camera.main.transform.position + Vector3.down;//* //verticalRaycastOffset;
        Vector3 rayDirection = playerCamera.forward; //Camera.main.transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.yellow);

        Ray gazeRay = new Ray(rayOrigin, rayDirection);
        if (Physics.Raycast(gazeRay, out RaycastHit hit, 100f))
        {
            if (hit.transform == transform)
            {
                hitPoint = hit.point;
                hitNormal = hit.normal;
                return true;
            }
        }

        hitPoint = Vector3.zero;
        hitNormal = Vector3.up;
        return false;
    }

    void Pop()
    {
        balloonRenderer.enabled = false;
        lookCrosshairCanvas.SetActive(false);

        if (poppedByPlayer && audioSource != null && !audioSource.isPlaying && scoreManager != null)
        {
            audioSource.Play();

            ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
                particleSystem.Play();

            Transform explosionModel = transform.Find("wlovo");
            if (explosionModel != null)
            {
                explosionModel.gameObject.SetActive(true);
                foreach (Transform fragment in explosionModel)
                {
                    Rigidbody rb = fragment.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;

                        Vector3 explosionDirection = (fragment.position - transform.position).normalized;
                        float force = Random.Range(150f, 300f);
                        rb.AddForce(explosionDirection * force);
                    }

                    fragment.localRotation = Quaternion.Euler(
                        Random.Range(0, 360),
                        Random.Range(0, 360),
                        Random.Range(0, 360)
                    );
                }
            }
            else
            {
                Debug.LogWarning("[BalloonBehaviour] No se encontr� el modelo de explosi�n 'wlovo'.");
            }

            scoreManager.SumarPunto();
        }
        
        // El globo se desactiva despu�s de explotar, y 1,5 segundos despu�s se desactiva el objeto
        Invoke(nameof(Deactivate), 1f);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void ReiniciarExplosion()
    {
        Transform explosionModel = transform.Find("wlovo");
        if (explosionModel != null)
        {
            explosionModel.gameObject.SetActive(false);
            foreach (Transform fragment in explosionModel)
            {
                Rigidbody rb = fragment.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                fragment.localPosition = Vector3.zero;
                fragment.localRotation = Quaternion.identity;
            }
        }
    }
}
