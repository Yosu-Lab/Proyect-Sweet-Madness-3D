
using UnityEngine;

public class ControladorPuzzleColores : MonoBehaviour
{
    public enum ColorPuzzle
    {
        Rojo,
        Azul,
        Amarillo,
        Verde,
        Morado,
        Naranja,
        Rosa,
        Blanco,
        Negro
    }

    [Header("Combinacion correcta")]
    public ColorPuzzle[] combinacionCorrecta =
    {
        ColorPuzzle.Rojo,
        ColorPuzzle.Azul,
        ColorPuzzle.Verde,
        ColorPuzzle.Amarillo
    };

    [Header("Puerta")]
    public GameObject puertaMorada;

    private int posicionActual = 0;

    public void PulsarRojo() => ComprobarColor(ColorPuzzle.Rojo);
    public void PulsarAzul() => ComprobarColor(ColorPuzzle.Azul);
    public void PulsarAmarillo() => ComprobarColor(ColorPuzzle.Amarillo);
    public void PulsarVerde() => ComprobarColor(ColorPuzzle.Verde);
    public void PulsarMorado() => ComprobarColor(ColorPuzzle.Morado);
    public void PulsarNaranja() => ComprobarColor(ColorPuzzle.Naranja);
    public void PulsarRosa() => ComprobarColor(ColorPuzzle.Rosa);
    public void PulsarNegro() => ComprobarColor(ColorPuzzle.Negro);
    public void PulsarBlanco() => ComprobarColor(ColorPuzzle.Blanco);

    private void ComprobarColor(ColorPuzzle colorPulsado)
    {
        if (combinacionCorrecta.Length == 0)
            return;

        if (colorPulsado == combinacionCorrecta[posicionActual])
        {
            posicionActual++;

            Debug.Log("¡Color correcto!");

            if (posicionActual >= combinacionCorrecta.Length)
            {
                ResolverPuzzle();
            }
        }
        else
        {
            Debug.Log("Combinación incorrecta. Vuelve a empezar.");
            posicionActual = 0;
        }
    }

    private void ResolverPuzzle()
    {
        Debug.Log("¡PUZZLE COMPLETADO!");

        if (puertaMorada != null)
        {
            puertaMorada.SetActive(false);
        }
    }
}
