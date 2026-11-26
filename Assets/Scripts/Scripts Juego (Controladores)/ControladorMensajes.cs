
using UnityEngine;
using TMPro;
using System.Collections;

public class ControladorMensajes : MonoBehaviour
{
    [Header("Referencia del texto")]
    public TMP_Text textoMensaje;

    [Header("Mensajes normales")]
    public float duracionMensaje = 4f;

    [Header("Mensaje final")]
    public float duracionMensajeFinal = 6f;
    public float tamañoMensajeFinal = 48f;

    private Coroutine corrutinaMensaje;
    private float tamañoOriginal;

    private void Start()
    {
        if (textoMensaje != null)
        {
            tamañoOriginal = textoMensaje.fontSize;
            textoMensaje.gameObject.SetActive(false);
        }
    }

    public void MostrarBuscaSalida()
    {
        MostrarMensaje("Busca la manera de salir de la fábrica");
    }

    public void MostrarRestablecerEnergia()
    {
        MostrarMensaje("Restablece la energía");
    }

    public void MostrarBuscaBateria()
    {
        MostrarMensaje("Busca una batería");
    }

    public void MostrarColocaBateria()
    {
        MostrarMensaje("Colócala en el generador");
    }

    public void MostrarHuyeFabrica()
    {
        MostrarMensaje("¡HUYE DE LA FÁBRICA!", true);
    }

    private void MostrarMensaje(string mensaje, bool mensajeFinal = false)
    {
        if (textoMensaje == null)
        {
            Debug.LogWarning("No hay texto asignado al ControladorMensajes.");
            return;
        }

        if (corrutinaMensaje != null)
        {
            StopCoroutine(corrutinaMensaje);
        }

        textoMensaje.fontSize = mensajeFinal
            ? tamañoMensajeFinal
            : tamañoOriginal;

        textoMensaje.text = mensaje;
        textoMensaje.gameObject.SetActive(true);

        float duracion = mensajeFinal
            ? duracionMensajeFinal
            : duracionMensaje;

        corrutinaMensaje = StartCoroutine(OcultarMensaje(duracion));
    }

    private IEnumerator OcultarMensaje(float duracion)
    {
        yield return new WaitForSeconds(duracion);

        textoMensaje.gameObject.SetActive(false);
        textoMensaje.fontSize = tamañoOriginal;

        corrutinaMensaje = null;
    }
}
