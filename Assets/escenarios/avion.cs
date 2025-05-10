using UnityEngine;

public class AdvancedAirplaneController : MonoBehaviour
{
    public Transform player;
    public float baseRadius = 4f;
    public float verticalAmplitude = 1f;
    public float verticalFrequency = 1f;
    public float minSpeed = 0.3f;
    public float maxSpeed = 0.6f;

    private float speed;
    private float time;
    private Vector3 lastPosition;

    private enum FlightMode { Orbit, Loop, Zigzag }
    private FlightMode currentMode;

    private float modeDuration = 5f;
    private float modeTimer = 0f;

    private Vector3 lastOffset;
    private Vector3 targetOffset;
    private float transitionTime = 1f;
    private float transitionTimer = 0f;
    private bool transitioning = false;

    void Start()
    {
        time = 0f;
        lastPosition = transform.position;
        SwitchFlightMode(true); // Primera vez, sin transición
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        modeTimer += Time.deltaTime;

        if (modeTimer >= modeDuration)
        {
            SwitchFlightMode();
        }

        Vector3 newOffset = ComputeCurrentOffset(time);

        // Si estamos en transición, interpolamos entre el último offset y el nuevo
        if (transitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionTime);
            newOffset = Vector3.Lerp(lastOffset, newOffset, SmoothStep(t));

            if (t >= 1f)
            {
                transitioning = false;
            }
        }

        Vector3 targetPos = player.position + newOffset;
        transform.position = targetPos;

        Vector3 direction = (transform.position - lastPosition).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        lastPosition = transform.position;
    }

    void SwitchFlightMode(bool instant = false)
    {
        lastOffset = ComputeCurrentOffset(time);
        currentMode = (FlightMode)Random.Range(0, 3);
        modeDuration = Random.Range(4f, 7f);
        speed = Random.Range(minSpeed, maxSpeed);
        modeTimer = 0f;

        if (!instant)
        {
            transitioning = true;
            transitionTimer = 0f;
        }
    }

    Vector3 ComputeCurrentOffset(float t)
    {
        switch (currentMode)
        {
            case FlightMode.Orbit:
                return OrbitPattern(t);
            case FlightMode.Loop:
                return LoopPattern(t);
            case FlightMode.Zigzag:
                return ZigzagPattern(t);
            default:
                return Vector3.zero;
        }
    }

    Vector3 OrbitPattern(float t)
    {
        float radius = baseRadius + Mathf.Sin(t * 0.5f) * 1.2f;
        float x = Mathf.Cos(t) * radius;
        float z = Mathf.Sin(t) * radius;
        float y = 1.5f + Mathf.Sin(t * verticalFrequency) * verticalAmplitude;
        return new Vector3(x, y, z);
    }

    Vector3 LoopPattern(float t)
    {
        float radius = baseRadius;
        float x = Mathf.Cos(t) * radius;
        float z = Mathf.Sin(t) * radius;
        float y = 1.5f + Mathf.Sin(t * 4f) * 2f;
        return new Vector3(x, y, z);
    }

    Vector3 ZigzagPattern(float t)
    {
        float radius = baseRadius + Mathf.PingPong(t, 1f);
        float x = Mathf.Cos(t) * radius;
        float z = Mathf.Sin(t) * radius;
        float y = 1.5f + Mathf.Sin(t * 6f) * 0.5f;
        return new Vector3(x + Mathf.Sin(t * 3f) * 0.8f, y, z);
    }

    float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t); // suaviza el Lerp para que no sea lineal
    }
}
