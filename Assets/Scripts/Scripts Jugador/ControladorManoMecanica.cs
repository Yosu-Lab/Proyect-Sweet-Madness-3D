
using UnityEngine;

public class ControladorManoMecanica : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;

    [Header("Distancia y velocidad")]
    public float distanciaMaxima = 8f;
    public float velocidadLanzamiento = 18f;
    public float velocidadRegreso = 18f;

    private Transform puntoAcoplamiento;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    private Vector3 destino;

    private Transform objetivoImpactado;
    private Vector3 posicionEngancheLocal;

    private Transform objetoTransportado;
    private Vector3 posicionObjetoLocal;
    private Quaternion rotacionObjetoLocal;

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

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (estado == EstadoMano.Preparada &&
                objetoTransportado == null)
            {
                LanzarMano();
            }
            else if (estado == EstadoMano.Enganchada)
            {
                IniciarRegreso();
            }
        }

        // Lanzamiento de la mano.
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

                if (objetivoValido && objetivoImpactado != null)
                {
                    if (objetivoImpactado.CompareTag("PuntoEnganche"))
                    {
                        // Guardamos el punto exacto del anclaje.
                        posicionEngancheLocal =
                            objetivoImpactado.InverseTransformPoint(
                                transform.position
                            );

                        estado = EstadoMano.Enganchada;
                    }
                    else if (objetivoImpactado.CompareTag("ObjetoAgarrable"))
                    {
                        // Recogemos el objeto sin cambiar su padre.
                        objetoTransportado = objetivoImpactado;

                        posicionObjetoLocal =
                            transform.InverseTransformPoint(
                                objetoTransportado.position
                            );

                        rotacionObjetoLocal =
                            Quaternion.Inverse(transform.rotation) *
                            objetoTransportado.rotation;

                        Rigidbody fisicas =
                            objetoTransportado.GetComponent<Rigidbody>();

                        if (fisicas != null)
                        {
                            fisicas.isKinematic = true;
                        }

                        IniciarRegreso();
                    }
                }
                else
                {
                    IniciarRegreso();
                }
            }
        }

        // Mano enganchada a un punto fijo.
        if (estado == EstadoMano.Enganchada)
        {
            if (objetivoImpactado != null)
            {
                transform.position =
                    objetivoImpactado.TransformPoint(
                        posicionEngancheLocal
                    );
            }
            else
            {
                IniciarRegreso();
            }
        }

        // Regreso de la mano.
        if (estado == EstadoMano.Regresando)
        {
            Vector3 posicionObjetivo =
                puntoAcoplamiento.TransformPoint(posicionInicial);

            transform.position = Vector3.MoveTowards(
                transform.position,
                posicionObjetivo,
                velocidadRegreso * Time.deltaTime
            );

            if (Vector3.Distance(
                transform.position,
                posicionObjetivo) < 0.05f)
            {
                transform.localPosition = posicionInicial;
                transform.localRotation = rotacionInicial;

                objetivoImpactado = null;
                estado = EstadoMano.Preparada;
            }
        }

        // El objeto permanece sujeto a la mano.
        if (objetoTransportado != null)
        {
            objetoTransportado.position =
                transform.TransformPoint(posicionObjetoLocal);

            objetoTransportado.rotation =
                transform.rotation * rotacionObjetoLocal;
        }
    }

    void LanzarMano()
    {
        if (camara == null)
        {
            return;
        }

        RaycastHit[] impactos = Physics.RaycastAll(
            camara.position,
            camara.forward,
            distanciaMaxima
        );

        objetivoValido = false;
        objetivoImpactado = null;

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

                Transform objeto = impacto.collider.transform;

                // Buscamos el Tag también en los padres.
                while (objeto != null &&
                       !objeto.CompareTag("PuntoEnganche") &&
                       !objeto.CompareTag("ObjetoAgarrable"))
                {
                    objeto = objeto.parent;
                }

                if (objeto != null)
                {
                    objetivoValido = true;
                    objetivoImpactado = objeto;
                }
            }
        }

        estado = EstadoMano.Lanzandose;
    }

    void IniciarRegreso()
    {
        objetivoImpactado = null;
        estado = EstadoMano.Regresando;
    }
}
