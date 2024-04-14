using UnityEngine;
using UnityEngine.UI;

public class CicloDiaNoche : MonoBehaviour
{
    public bool isManager = false;
    public Light luz;
    public Image pantallaNegra;
    public Text textoDia;
    public Transform Agentes;
    public float transicion = 1;
    private int numIteraciones = 1;

    void Start()
    {
        bool isMultiSimulation = GetComponent<NavMUpdate>().multiSimulation;

        if(isManager) 
        {
            Application.targetFrameRate = 60;
        }

        if(isMultiSimulation) InvokeRepeating("IniciarCicloMultiSim", 0.1f, 0.1f);
        else
        {
            FadeInterfaz(true);
            InvokeRepeating("IniciarNoche", 0f, 60f);
            InvokeRepeating("IniciarDia", 0f, 80f);
        }
    }

    void IniciarCicloMultiSim()
    {
        if(GetComponent<MultiSimulation>().iniciarCicloDiario)
        {
            GetComponent<MultiSimulation>().iniciarCicloDiario = false;
            FadeInterfaz(true);
            InvokeRepeating("IniciarNoche", 0f, 60f);
            InvokeRepeating("IniciarDia", 0f, 80f);
        }
    }

    void IniciarDia()
    {
        if(isManager)
        {
            textoDia.text = "Día " + numIteraciones + "  Semilla: " + Util.seed;
            FadeInterfaz(false);
            luz.enabled = true;
            FadeInterfaz(true); Debug.Log("Día " + numIteraciones);
        }
        
        numIteraciones++;

        // Si han pasado 3 días, reiniciar la escena
        if (numIteraciones > 4 && isManager)
        {
            Debug.Log("== Simulacion Multiple Ejecutada ==");
            ReiniciarEscena();
        }
    }

    void IniciarNoche() 
    { 
        if(isManager) luz.enabled = false; 
        
        if (numIteraciones == 4 && Application.isEditor && !isManager)
        {
            Invoke("GuardarSimData", Random.Range(1, 3));
        }
    }

    void GuardarSimData()
    {
        foreach (Transform Agente in Agentes) Agente.GetComponent<DatosEntidad>().GuardarResultados();
    }

    void FadeInterfaz(bool activarFade)
    {
        int alfa = activarFade ? 0 : 1;
        float fadeTime = activarFade ? transicion : 0;

        pantallaNegra.CrossFadeAlpha(alfa, fadeTime, false);
        textoDia.CrossFadeAlpha(alfa, fadeTime, false);
    }

    void ReiniciarEscena()
    {
        // Resetear la escena liberando recursos
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
