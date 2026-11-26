
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia;

    [Header("Audio Source")]
    public AudioSource efectosAudio;

    [Header("Sonidos Puzzle Colores")]
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    public AudioClip sonidoCompletado;

    [Header("Sonidos Generador")]
    public AudioClip sonidoElectricidad;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReproducirCorrecto()
    {
        if (efectosAudio != null && sonidoCorrecto != null)
            efectosAudio.PlayOneShot(sonidoCorrecto);
    }

    public void ReproducirIncorrecto()
    {
        if (efectosAudio != null && sonidoIncorrecto != null)
            efectosAudio.PlayOneShot(sonidoIncorrecto);
    }

    public void ReproducirCompletado()
    {
        if (efectosAudio != null && sonidoCompletado != null)
            efectosAudio.PlayOneShot(sonidoCompletado);
    }

    public void ReproducirElectricidad()
    {
        if (efectosAudio != null && sonidoElectricidad != null)
            efectosAudio.PlayOneShot(sonidoElectricidad);
    }
}
