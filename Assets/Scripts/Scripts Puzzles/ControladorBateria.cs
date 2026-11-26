

using UnityEngine;

public class ControladorBateria : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject bateria;
    public Transform puntoBateriaManos;
    public Transform ranuraBateria;

    [Header("Control del brazo mecanico")]
    public ControladorManoMecanica controladorMano;

    [Header("Mensajes")]
    public ControladorMensajes controladorMensajes;

    private bool tieneBateria = false;
    private bool bateriaInsertada = false;

    private Collider[] collidersBateria;
    private Rigidbody fisicaBateria;

    private void Start()
    {
        collidersBateria = bateria.GetComponentsInChildren<Collider>();
        fisicaBateria = bateria.GetComponent<Rigidbody>();
    }

    // RECOGER BATERIA
    public void RecogerBateria()
    {
        if (tieneBateria || bateriaInsertada)
            return;

        tieneBateria = true;

        // Desactivar las colisiones de la bateria
        foreach (Collider col in collidersBateria)
        {
            col.enabled = false;
        }

        // Desactivar la fisica
        if (fisicaBateria != null)
        {
            fisicaBateria.isKinematic = true;
            fisicaBateria.useGravity = false;
        }

        // Colocar la bateria delante de la camara
        bateria.transform.SetParent(puntoBateriaManos);
        bateria.transform.localPosition = Vector3.zero;
        bateria.transform.localRotation = Quaternion.identity;

        // Bloquear el uso del brazo mecanico
        if (controladorMano != null)
        {
            controladorMano.EstablecerUsoBloqueado(true);
        }

        Debug.Log("Bateria recogida. Brazo mecanico bloqueado.");
    }

    // INSERTAR BATERIA
    public void IntentarInsertarBateria()
    {
        if (!tieneBateria || bateriaInsertada)
            return;

        bateriaInsertada = true;
        tieneBateria = false;

        // Colocar la bateria dentro de la ranura
        bateria.transform.SetParent(ranuraBateria);
        bateria.transform.localPosition = Vector3.zero;
        bateria.transform.localRotation = Quaternion.identity;

        // Desbloquear el uso del brazo mecanico
        if (controladorMano != null)
        {
            controladorMano.EstablecerUsoBloqueado(false);
        }

        // Reproducir sonido de electricidad
        if (AudioManager.Instancia != null)
        {
            AudioManager.Instancia.ReproducirElectricidad();
        }

        // Mostrar mensaje final
        if (controladorMensajes != null)
        {
            controladorMensajes.MostrarHuyeFabrica();
        }

        Debug.Log("Bateria insertada. Energia restablecida. ¡Huye de la fabrica!");
    }
}
