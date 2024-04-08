using UnityEngine;
using UnityEngine.UI;

public class CicloDiaNoche : MonoBehaviour
{
    public Light luz;
    public Image pantallaNegra;
    public Text textoDia;
    public Transform Agentes;
    public float transicion = 1;
    private int numIteraciones = 1;
    //int seed;

    void Start()
    {
        //seed = System.Environment.TickCount;//textoDia.text = "Día 0  Semilla: " + System.Environment.TickCount;
        FadeInterfaz(true);
        InvokeRepeating("IniciarNoche", 0f, 60f);
        InvokeRepeating("IniciarDia", 0f, 80f);
    }

    void IniciarDia()
    {
        textoDia.text = "Día " + numIteraciones + "  Semilla: " + Util.seed;
        FadeInterfaz(false);
        luz.enabled = true;
        FadeInterfaz(true); Debug.Log("Día " + numIteraciones);
        numIteraciones++;

        // Si han pasado 3 días, reiniciar la escena
        if (numIteraciones > 4)
        {
            ReiniciarEscena();
        }
    }

    void IniciarNoche() 
    { 
        luz.enabled = false; 
        
        if (numIteraciones == 4 && Application.isEditor)
        {
            foreach (Transform Agente in Agentes) 
                Agente.GetComponent<DatosEntidad>().GuardarResultados();
            
            Debug.Log("== Simulacion " + Util.seed + " Guardada ==");
        }
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
