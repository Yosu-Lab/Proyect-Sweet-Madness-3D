
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform playerCamera;
    public ControladorManoMecanica controladorMano;

    [Header("Movimiento")]
    public float velocidadmovimiento = 5f;
    public float VelocidadSprint = 10f;
    public float AlturaSalto = 2f;
    public float gravedad = -20f;

    [Header("Camara")]
    public float SensibildadRaton = 2f;
    public float AnguloMaximo = 85f;

    [Header("Cable mecanico")]
    public float longitudCable = 10f;

    [Header("Balanceo")]
    public float fuerzaBalanceo = 18f;
    public float velocidadMaximaBalanceo = 15f;
    public float amortiguacionBalanceo = 0.15f;

    [Header("Impulso al soltar")]
    public float friccionSalida = 3f;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;

    private bool estabaEnganchado;

    private Transform puntoBalanceo;
    private float longitudBalanceo;

    private Vector3 velocidadBalanceo;
    private Vector3 velocidadSalida;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();

        bool enganchado =
            controladorMano != null &&
            controladorMano.EstaEnganchada;

        if (enganchado)
        {
            if (!estabaEnganchado)
            {
                IniciarBalanceo();
            }

            MoverBalanceo();
        }
        else
        {
            if (estabaEnganchado)
            {
                velocidadSalida = velocidadBalanceo;
                velocidadBalanceo = Vector3.zero;
                puntoBalanceo = null;
            }

            Move();
        }

        estabaEnganchado = enganchado;
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * SensibildadRaton;
        float mouseY = Input.GetAxis("Mouse Y") * SensibildadRaton;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(
            cameraPitch, -AnguloMaximo, AnguloMaximo
        );

        playerCamera.localRotation =
            Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            (transform.right * x + transform.forward * z).normalized;

        float currentSpeed = velocidadmovimiento;

        if (Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = VelocidadSprint;
        }

        if (grounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity =
                Mathf.Sqrt(AlturaSalto * -2f * gravedad);
        }

        verticalVelocity += gravedad * Time.deltaTime;

        controller.Move(
            direction * currentSpeed * Time.deltaTime
            + Vector3.up * verticalVelocity * Time.deltaTime
            + velocidadSalida * Time.deltaTime
        );

        velocidadSalida = Vector3.MoveTowards(
            velocidadSalida,
            Vector3.zero,
            friccionSalida * Time.deltaTime
        );
    }

    void IniciarBalanceo()
    {
        puntoBalanceo =
            controladorMano.PuntoEngancheActual;

        velocidadBalanceo = Vector3.zero;

        if (puntoBalanceo == null)
            return;

        longitudBalanceo = Mathf.Min(
            Vector3.Distance(transform.position, puntoBalanceo.position),
            longitudCable
        );

        longitudBalanceo = Mathf.Max(1f, longitudBalanceo);
    }

    void MoverBalanceo()
    {
        if (puntoBalanceo == null)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direccionEntrada =
            transform.right * x + transform.forward * z;

        Vector3 diferencia =
            transform.position - puntoBalanceo.position;

        Vector3 direccionRadial = diferencia.normalized;

        // Solo permitimos movimiento tangencial a la cuerda.
        Vector3 direccionTangencial =
            Vector3.ProjectOnPlane(
                direccionEntrada,
                direccionRadial
            ).normalized;

        // La gravedad también genera el balanceo.
        Vector3 gravedadTangencial =
            Vector3.ProjectOnPlane(
                Physics.gravity,
                direccionRadial
            );

        Vector3 aceleracion =
            direccionTangencial * fuerzaBalanceo
            + gravedadTangencial;

        velocidadBalanceo +=
            aceleracion * Time.deltaTime;

        velocidadBalanceo = Vector3.Lerp(
            velocidadBalanceo,
            Vector3.zero,
            amortiguacionBalanceo * Time.deltaTime
        );

        // Evitamos superar la velocidad máxima.
        velocidadBalanceo = Vector3.ClampMagnitude(
            velocidadBalanceo,
            velocidadMaximaBalanceo
        );

        // Eliminamos el movimiento que se aleja radialmente.
        velocidadBalanceo = Vector3.ProjectOnPlane(
            velocidadBalanceo,
            direccionRadial
        );

        controller.Move(
            velocidadBalanceo * Time.deltaTime
        );

        // Mantenemos la longitud de la cuerda.
        diferencia = transform.position - puntoBalanceo.position;

        if (diferencia.magnitude > 0.01f)
        {
            Vector3 posicionPermitida =
                puntoBalanceo.position +
                diferencia.normalized * longitudBalanceo;

            Vector3 correccion =
                posicionPermitida - transform.position;

            controller.Move(correccion);
        }
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
