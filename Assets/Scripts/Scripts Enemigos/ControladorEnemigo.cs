
using UnityEngine;
using UnityEngine.AI;

public class ControladorEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public Transform[] puntosPatrulla;

    [Header("Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 5f;
    public float distanciaParada = 0.5f;

    [Header("Deteccion")]
    public float distanciaVision = 12f;
    public float distanciaDeteccionCercana = 3.5f;

    [Range(0f, 360f)]
    public float anguloVision = 110f;

    public float alturaOjos = 1.4f;
    public float alturaJugador = 1f;

    [Header("Busqueda")]
    public float tiempoBusqueda = 4f;

    private NavMeshAgent agente;
    private int indicePatrulla = 0;

    private bool persiguiendo = false;
    private float tiempoSinVerJugador = 0f;

    private Vector3 ultimaPosicionJugador;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        agente.speed = velocidadPatrulla;
        agente.stoppingDistance = distanciaParada;

        // Configuracion del giro.
        agente.angularSpeed = 720f;
        agente.acceleration = 20f;

        if (puntosPatrulla != null &&
            puntosPatrulla.Length > 0)
        {
            IrAlPuntoActual();
        }
    }

    void Update()
    {
        if (jugador == null || !agente.isOnNavMesh)
            return;

        if (PuedeVerJugador())
        {
            persiguiendo = true;
            tiempoSinVerJugador = 0f;

            ultimaPosicionJugador = jugador.position;

            agente.speed = velocidadPersecucion;
            agente.SetDestination(jugador.position);
        }
        else if (persiguiendo)
        {
            tiempoSinVerJugador += Time.deltaTime;

            agente.SetDestination(ultimaPosicionJugador);

            // Cuando llega a la ultima posicion,
            // comienza a buscar durante unos segundos.
            if (tiempoSinVerJugador >= tiempoBusqueda)
            {
                persiguiendo = false;

                agente.speed = velocidadPatrulla;

                SiguientePunto();
            }
        }
        else
        {
            Patrullar();
        }
    }

    bool PuedeVerJugador()
    {
        Vector3 origen = transform.position +
                         Vector3.up * alturaOjos;

        Vector3 destino = jugador.position +
                          Vector3.up * alturaJugador;

        Vector3 direccion = destino - origen;

        float distanciaCercana = Vector3.Distance(
            transform.position,
            jugador.position
        );

        float distanciaRayo = direccion.magnitude;

        // Comprobar distancia maxima.
        if (distanciaCercana > distanciaVision)
            return false;

        // Comprobar angulo de vision.
        float angulo = Vector3.Angle(
            transform.forward,
            direccion
        );

        // Si esta detras, solo detectarlo si esta cerca.
        if (angulo > anguloVision / 2f &&
            distanciaCercana > distanciaDeteccionCercana)
        {
            return false;
        }

        // Comprobar si hay obstaculos.
        if (Physics.Raycast(
            origen,
            direccion.normalized,
            out RaycastHit impacto,
            distanciaRayo + 0.2f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            if (impacto.transform == jugador ||
                impacto.transform.IsChildOf(jugador))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    void Patrullar()
    {
        if (puntosPatrulla == null ||
            puntosPatrulla.Length == 0)
            return;

        if (!agente.pathPending &&
            agente.remainingDistance <=
            agente.stoppingDistance + 0.1f)
        {
            SiguientePunto();
        }
    }

    void SiguientePunto()
    {
        if (puntosPatrulla == null ||
            puntosPatrulla.Length == 0)
            return;

        indicePatrulla++;

        if (indicePatrulla >= puntosPatrulla.Length)
        {
            indicePatrulla = 0;
        }

        IrAlPuntoActual();
    }

    void IrAlPuntoActual()
    {
        if (puntosPatrulla[indicePatrulla] != null)
        {
            agente.SetDestination(
                puntosPatrulla[indicePatrulla].position
            );
        }
    }
}
