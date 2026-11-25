
using UnityEngine;
using TMPro;

public class DetectorInteraccion : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camara;
    public TMP_Text textoInteraccion;

    [Header("Deteccion")]
    public float distanciaInteraccion = 5f;

    private Interactuable objetoActual;
    private Interactuable objetoAnterior;

    void Start()
    {
        textoInteraccion.gameObject.SetActive(false);
    }

    void Update()
    {
        DetectarObjeto();

        if (objetoActual != null &&
            Input.GetKeyDown(KeyCode.E))
        {
            objetoActual.Interactuar();
        }
    }

    void DetectarObjeto()
    {
        objetoActual = null;

        RaycastHit[] impactos = Physics.RaycastAll(
            camara.transform.position,
            camara.transform.forward,
            distanciaInteraccion
        );

        float distanciaCercana = distanciaInteraccion;

        foreach (RaycastHit impacto in impactos)
        {
            // Ignorar los objetos del propio jugador.
            if (impacto.collider.transform == transform.root ||
                impacto.collider.transform.IsChildOf(transform.root))
            {
                continue;
            }

            Interactuable interactuable =
                impacto.collider.GetComponentInParent<Interactuable>();

            if (interactuable != null &&
                impacto.distance < distanciaCercana)
            {
                distanciaCercana = impacto.distance;
                objetoActual = interactuable;
            }
        }

        // Solo actualizamos la interfaz cuando cambia el objeto.
        if (objetoActual != objetoAnterior)
        {
            if (objetoActual != null)
            {
                textoInteraccion.text = objetoActual.mensaje;
                textoInteraccion.gameObject.SetActive(true);
            }
            else
            {
                textoInteraccion.gameObject.SetActive(false);
            }

            objetoAnterior = objetoActual;
        }
    }
}
