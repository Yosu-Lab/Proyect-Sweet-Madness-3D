
using UnityEngine;

public class ZonaMensajesGenerador : MonoBehaviour
{
    public enum TipoMensaje
    {
        BuscaSalida,
        RestablecerEnergia,
        BuscaBateria,
        ColocaBateria
    }

    [Header("Configuracion de la zona")]
    public TipoMensaje tipoMensaje;

    [Header("Controlador de mensajes")]
    public ControladorMensajes controladorMensajes;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado)
            return;

        if (other.GetComponentInParent<CharacterController>() == null)
            return;

        if (controladorMensajes == null)
        {
            Debug.LogWarning("Falta asignar el ControladorMensajes.");
            return;
        }

        activado = true;

        switch (tipoMensaje)
        {
            case TipoMensaje.BuscaSalida:
                controladorMensajes.MostrarBuscaSalida();
                break;

            case TipoMensaje.RestablecerEnergia:
                controladorMensajes.MostrarRestablecerEnergia();
                break;

            case TipoMensaje.BuscaBateria:
                controladorMensajes.MostrarBuscaBateria();
                break;

            case TipoMensaje.ColocaBateria:
                controladorMensajes.MostrarColocaBateria();
                break;
        }
    }
}
