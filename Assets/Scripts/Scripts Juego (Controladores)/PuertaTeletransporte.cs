
using UnityEngine;

public class PuertaTeletransporte : MonoBehaviour
{
    [Header("Destino")]
    public Transform puntoEntradaFabrica;

    [Header("Jugador")]
    public Transform jugador;

    private bool jugadorCerca = false;

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            EntrarFabrica();
        }
    }

    private void EntrarFabrica()
    {
        if (puntoEntradaFabrica == null || jugador == null)
        {
            Debug.LogWarning("Falta asignar el jugador o el destino.");
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == jugador)
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == jugador)
        {
            jugadorCerca = false;
        }
    }
}
