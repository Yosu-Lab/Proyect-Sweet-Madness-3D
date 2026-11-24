
using UnityEngine;

public class ControladorManoMecanica : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;

    [Header("Distancia y velocidad")]
    public float distanciaMaxima = 5f;
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

    public bool EstaEnganchada =>
        estado == EstadoMano.Enganchada;

    public Transform PuntoEngancheActual =>
        objetivoImpactado;

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
        // Lanzar o recuperar la mano.
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

        // Soltar el objeto con el botón derecho.
        if (Input.GetMouseButtonDown(1) &&
            objetoTransportado != null)
        {
            SoltarObjeto();
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
                        posicionEngancheLocal =
                            objetivoImpactado.InverseTransformPoint(
                                transform.position
                            );

                        estado = EstadoMano.Enganchada;
                    }
                    else if (objetivoImpactado.CompareTag("ObjetoAgarrable"))
                    {
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

    void SoltarObjeto()
    {
        if (objetoTransportado == null)
            return;

        Rigidbody fisicas =
            objetoTransportado.GetComponent<Rigidbody>();

        if (fisicas != null)
        {
            fisicas.isKinematic = false;
        }

        objetoTransportado = null;
    }
}
