using UnityEngine;

public class CanastaDetector : MonoBehaviour
{
    public ContadorPuntos contador;
    public int puntos = 1;

    public float cooldownTiempo = 1.0f;
    private bool enCooldown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (enCooldown)
            return;

        if (other.CompareTag("Balon"))
        {
            switch (puntos)
            {
                case 1:
                    contador.SumarPunto();
                    break;
                case 2:
                    contador.SumarDoble();
                    break;
                case 5:
                    contador.Sumar5();
                    break;
            }

            // Iniciar cooldown
            enCooldown = true;
            Invoke(nameof(ReiniciarCooldown), cooldownTiempo);
        }
    }

    private void ReiniciarCooldown()
    {
        enCooldown = false;
    }
}
