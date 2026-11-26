
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    [Header("Panel de pausa")]
    public GameObject panelPausa;

    [Header("Escena del menu principal")]
    public string nombreEscenaMenu = "MenuPrincipal";

    private bool juegoPausado = false;

    private void Start()
    {
        panelPausa.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
                ContinuarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;

        panelPausa.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinuarJuego()
    {
        juegoPausado = false;

        panelPausa.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
