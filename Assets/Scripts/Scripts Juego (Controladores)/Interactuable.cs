
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour
{
    [Header("Interaccion")]
    public string mensaje = "E - INTERACTUAR";

    [Header("Evento")]
    public UnityEvent alInteractuar;

    public void Interactuar()
    {
        alInteractuar.Invoke();
    }
}
