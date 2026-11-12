
using UnityEngine;

public class ControladorManoMecanica : MonoBehaviour
{
    public Transform camara;

    public float distanciaMaxima = 8f;
    public float velocidadLanzamiento = 18f;
    public float velocidadRegreso = 18f;

    private Transform puntoAcoplamiento;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    private Vector3 destino;
    private Vector3 posicionEnganche;

    private bool objetivoValido;

    private EstadoMano estado = EstadoMano.Preparada;

    private enum EstadoMano
    {
        Preparada,
        Lanzandose,
        Enganchada,
        Regresando
    }

    void Start()
    {
        puntoAcoplamiento = transform.parent;

        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;

        if (camara == null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (estado == EstadoMano.Preparada)
            {
                LanzarMano();
            }
            else if (estado == EstadoMano.Enganchada)
            {
                IniciarRegreso();
            }
        }

        if (estado == EstadoMano.Lanzandose)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadLanzamiento * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, destino) < 0.05f)
            {
                transform.position = destino;

                if (objetivoValido)
                {
                    estado = EstadoMano.Enganchada;
                }
                else
                {
                    IniciarRegreso();
                }
            }
        }

        if (estado == EstadoMano.Regresando)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                puntoAcoplamiento.position,
                velocidadRegreso * Time.deltaTime
            );

            if (Vector3.Distance(
                transform.position,
                puntoAcoplamiento.position) < 0.05f)
            {
                transform.localPosition = posicionInicial;
                transform.localRotation = rotacionInicial;

                estado = EstadoMano.Preparada;
            }
        }
    }


    void LanzarMano()
    {
        RaycastHit[] impactos = Physics.RaycastAll(
            camara.position,
            camara.forward,
            distanciaMaxima
        );

        objetivoValido = false;

        float distanciaCercana = distanciaMaxima;

        destino = camara.position +
                  camara.forward * distanciaMaxima;

        foreach (RaycastHit impacto in impactos)
        {
            if (impacto.collider.transform == camara.root ||
                impacto.collider.transform.IsChildOf(camara.root))
            {
                continue;
            }

            if (impacto.distance < distanciaCercana)
            {
                distanciaCercana = impacto.distance;
                destino = impacto.point;

                objetivoValido =
                    impacto.collider.CompareTag("ObjetivoMano");
            }
        }

        estado = EstadoMano.Lanzandose;
    }

    void IniciarRegreso()
    {
        estado = EstadoMano.Regresando;
    }
}
