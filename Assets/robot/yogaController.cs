using UnityEngine;

public class YogaCharacterController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    public AudioClip audioBienvenida;
    public AudioClip audioBrazos;
    public AudioClip audioUno;
    public AudioClip audioDos;

    public float duracionAnimacionYoga = 6f; // duración deseada de cada ciclo de yoga y alzar brazos
    public float duracionOriginalYoga = 3.967f; // duración real de la animación Yoga
    public float duracionOriginalAlzarBrazos = 3.967f; // duración real de AlzarBrazos

    public GameTimer gameTimer;

    private enum Estado { Esperando, Aparicion, Bienvenida, Torsion, Yoga, EsperandoBrazos, AlzarBrazos, Finalizado }
    private Estado estadoActual = Estado.Esperando;

    private bool yaEjecutadoCambioMitad = false;

    private float animTimer = 0f;
    private bool haReproducidoUno = false;
    private bool haReproducidoDos = false;

    void Update()
    {
        if (gameTimer == null) return;

        if (estadoActual == Estado.Esperando)
        {
            IniciarAparicion();
            return;
        }

        if (gameTimer.EstaPausado())
        {
            animator.speed = 0f;
            audioSource.Pause();
            return;
        }
        else
        {
            audioSource.UnPause();
            if (estadoActual != Estado.Yoga)
            {
                animator.speed = 1f; // restaurar velocidad de animación
            }
            else
            {
                animator.speed = duracionOriginalYoga / duracionAnimacionYoga; // ajustar velocidad de animación
            }
        }

        float tiempoRestante = gameTimer.TiempoRestante();
        float mitadTiempo = gameTimer.DuracionTotal() / 2f;

        if (gameTimer.InitialDelayRestante() <= 0f)
        {
            switch (estadoActual)
            {
                case Estado.Yoga:
                    ControlarAudioContar(duracionAnimacionYoga);
                    break;
                case Estado.AlzarBrazos:
                    ControlarAudioContar(duracionOriginalAlzarBrazos);
                    break;
            }

            if (estadoActual == Estado.Yoga && !yaEjecutadoCambioMitad && tiempoRestante <= mitadTiempo)
            {
                yaEjecutadoCambioMitad = true;
                CambiarABrazos();
            }
        }

        if (tiempoRestante <= 0f && estadoActual != Estado.Finalizado)
        {
            FinalizarSesion();
        }
    }

    void ControlarAudioContar(float dur)
    {
        animTimer += Time.deltaTime;

        if (!haReproducidoUno && animTimer >= dur * 1/4f)
        {
            audioSource.PlayOneShot(audioUno);
            haReproducidoUno = true;
        }

        if (!haReproducidoDos && animTimer >= dur * 3/4f)
        {
            audioSource.PlayOneShot(audioDos);
            haReproducidoDos = true;
        }

        if (animTimer >= dur)
        {
            animTimer = 0f;
            haReproducidoUno = false;
            haReproducidoDos = false;
        }
    }

    void IniciarAparicion()
    {
        estadoActual = Estado.Aparicion;
        animator.Play("Aparicion");
        Invoke(nameof(IniciarBienvenida), 3.28f);
    }

    void IniciarBienvenida()
    {
        estadoActual = Estado.Bienvenida;
        animator.Play("Bienvenida");

        if (audioBienvenida != null)
        {
            audioSource.clip = audioBienvenida;
            audioSource.loop = false;
            audioSource.Play();
            Invoke(nameof(IniciarTorsion), audioBienvenida.length * 0.75f);
        }
        else
        {
            Debug.LogWarning("Audio de bienvenida no asignado.");
            Invoke(nameof(IniciarTorsion), 2f);
        }
    }

    void IniciarTorsion()
    {
        estadoActual = Estado.Torsion;
        animator.Play("Torsion");
        Invoke(nameof(IniciarYoga), 3.28f);
    }

    void IniciarYoga()
    {
        estadoActual = Estado.Yoga;
        animator.Play("Yoga");

        // Ajustar velocidad de animación para que dure duracionAnimacionYoga
        animator.speed = duracionOriginalYoga / duracionAnimacionYoga;

        animTimer = 0f;
        haReproducidoUno = false;
        haReproducidoDos = false;
    }

    void CambiarABrazos()
    {
        estadoActual = Estado.EsperandoBrazos;
        audioSource.Stop();
        animator.Play("Idle");
        animator.speed = 1f; // restaurar velocidad para transición

        if (audioBrazos != null)
        {
            audioSource.clip = audioBrazos;
            audioSource.loop = false;
            audioSource.Play();
            Invoke(nameof(IniciarAlzarBrazos), audioBrazos.length);
        }
        else
        {
            Debug.LogWarning("Audio 'brazos' no asignado.");
            Invoke(nameof(IniciarAlzarBrazos), 2f);
        }
    }

    void IniciarAlzarBrazos()
    {
        estadoActual = Estado.AlzarBrazos;
        animator.Play("AlzarBrazos");

        // Ajustar velocidad para que la animación dure duracionAnimacionYoga
        //animator.speed = duracionOriginalAlzarBrazos / duracionAnimacionYoga;

        animTimer = 0f;
        haReproducidoUno = false;
        haReproducidoDos = false;
    }

    void FinalizarSesion()
    {
        estadoActual = Estado.Finalizado;
        audioSource.Stop();
        animator.Play("Idle");
        animator.speed = 1f; // restaurar velocidad
    }
}
