using UnityEngine;
using System;

public class AgenteLogros : MonoBehaviour
{
    private int puntosAcumuladosEstaSemana = 0;
    private int puntosMinimos = 50;

    // Para controlar que solo se actualice 1 vez por semana
    private int semanaUltimaActualizacion = -1;


    // metodo que llama usuarioglobal tras cerrar sesion
    public void ReiniciarLogroSemanal()
    {
        semanaUltimaActualizacion = -1;
        puntosAcumuladosEstaSemana = 0;
        Debug.Log("[AgenteLogros] Logro semanal reiniciado.");
    }

    private void OnEnable()
    {
        ContadorPuntos.OnPuntosGanados += OnPuntosGanados;
    }

    private void OnDisable()
    {
        ContadorPuntos.OnPuntosGanados -= OnPuntosGanados;
    }

    private void OnPuntosGanados(int puntos)
    {
        puntosAcumuladosEstaSemana += puntos;
        Debug.Log($"[AgenteLogros] Puntos acumulados esta semana: {puntosAcumuladosEstaSemana}");

        if (puntosAcumuladosEstaSemana >= puntosMinimos)
        {
            int semanaActual = GetSemanaDelAño(DateTime.UtcNow);

            if (semanaActual != semanaUltimaActualizacion)
            {
                semanaUltimaActualizacion = semanaActual;
                puntosAcumuladosEstaSemana = 0; // Reiniciar para próxima semana

                AgregarOActualizarLogroSemanal();
            }
            else
            {
                Debug.Log("[AgenteLogros] El logro semanal ya fue actualizado esta semana.");
            }
        }
    }

    private void AgregarOActualizarLogroSemanal()
    {
        var usuarioGlobal = UsuarioGlobal.Instance;
        if (usuarioGlobal == null || usuarioGlobal.UsuarioActual == null)
        {
            Debug.LogWarning("[AgenteLogros] No hay usuario actual para actualizar el logro.");
            return;
        }

        var logroSemanal = usuarioGlobal.UsuarioActual.logros.Find(l => l.id == 50);

        if (logroSemanal == null)
        {
            logroSemanal = new Logro(
                50,
                "Gana 50 puntos semanalmente",
                "Ha ganado más de 50 puntos en 1 semana",
                "/uploads/logro_semanal.png",
                DateTime.UtcNow.ToString("o"),
                1
            );
            usuarioGlobal.AgregarLogro(logroSemanal);
            Debug.Log("[AgenteLogros] Logro semanal creado.");
        }
        else
        {
            logroSemanal.semanasCumplidas++;
            logroSemanal.description = $"Ha ganado más de 50 puntos en {logroSemanal.semanasCumplidas} semanas";
            logroSemanal.dateAchieved = DateTime.UtcNow.ToString("o");
            Debug.Log("[AgenteLogros] Logro semanal actualizado: " + logroSemanal.description);
        }

        // Emitir sonido de logro del AudioSource
        

        // Aquí podrías agregar llamada para sincronizar con servidor si tienes API
    }

    // Devuelve la semana del año según fecha (ISO 8601)
    private int GetSemanaDelAño(DateTime fecha)
    {
        var cultura = System.Globalization.CultureInfo.CurrentCulture;
        return cultura.Calendar.GetWeekOfYear(fecha, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
}
