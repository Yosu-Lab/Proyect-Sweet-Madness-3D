
using UnityEngine;

public class TeletransportePuerta : MonoBehaviour
{
    [Header("Configuración")]
    public Transform jugador;
    public Transform puntoEntradaFabrica;

    public void EntrarFabrica()
    {
        if (jugador == null || puntoEntradaFabrica == null)
        {
            Debug.LogWarning("Faltan referencias del teletransporte.");
            return;
        }

        CharacterController controlador =
            jugador.GetComponent<CharacterController>();

        if (controlador != null)
            controlador.enabled = false;

        jugador.SetPositionAndRotation(
            puntoEntradaFabrica.position,
            puntoEntradaFabrica.rotation
        );

        if (controlador != null)
            controlador.enabled = true;
    }
}
