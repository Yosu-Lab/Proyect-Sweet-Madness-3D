
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Equipamiento del jugador")]
    public GameObject controladorBrazo;

    [Header("Objetos del escenario")]
    public GameObject brazoRecogible;

    private bool tieneBrazo = false;

    void Start()
    {
        // El jugador comienza sin el brazo.
        controladorBrazo.SetActive(false);

        // El brazo está disponible en el escenario.
        brazoRecogible.SetActive(true);
    }

    public void RecogerBrazo()
    {
        if (tieneBrazo)
            return;

        tieneBrazo = true;

        brazoRecogible.SetActive(false);
        controladorBrazo.SetActive(true);

        Debug.Log("Brazo mecánico equipado.");
    }
}
