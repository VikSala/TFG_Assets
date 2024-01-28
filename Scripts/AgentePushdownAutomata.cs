using System.Collections.Generic;
using UnityEngine;

// Enumeración de los estados posibles del agente
public enum EstadoAgenteExistencia
{
    SinNecesidad, 
    Hambre,     //El agente tiene hambre 
    Sed,        //El agente tiene sed 
    Somnolencia,    //El agente tiene sueño 
    Amenaza,    //El agente detecta amenaza 
    Peligro     //El agente detecta peligro
}

public enum EstadoAgenteRealidad
{
    Recurso,    //El agente detecta un recurso
    Agente      //El agente detecta un agente
}

// Clase para manejar el estado actual y la pila de estados
public class ControladorEstados
{
    private EstadoAgenteExistencia estadoActual;
    private Stack<EstadoAgenteExistencia> pilaEstados = 
                    new Stack<EstadoAgenteExistencia>();
    private HashSet<EstadoAgenteExistencia> stateHashSet = 
                    new HashSet<EstadoAgenteExistencia>();

    public bool isDebug = false;

    public ControladorEstados(bool isDebug)
    {
        this.isDebug = isDebug;
        estadoActual = EstadoAgenteExistencia.SinNecesidad;
    }

    public bool CambiarEstado(EstadoAgenteExistencia nuevoEstado)
    {
        bool result = false;

        if(VerificarPrioridad(nuevoEstado)) result = true;

        if (!stateHashSet.Contains(nuevoEstado))
        {
            pilaEstados.Push(estadoActual);
            estadoActual = nuevoEstado;
            stateHashSet.Add(estadoActual);
        }
        return result;
    }

    private bool VerificarPrioridad(EstadoAgenteExistencia nuevoEstado)
    {
        switch (nuevoEstado)
        {
            case EstadoAgenteExistencia.Hambre:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza && 
                estadoActual != EstadoAgenteExistencia.Somnolencia;

            case EstadoAgenteExistencia.Sed:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza && 
                estadoActual != EstadoAgenteExistencia.Somnolencia;

            case EstadoAgenteExistencia.Somnolencia:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza;

            case EstadoAgenteExistencia.Amenaza:
                return estadoActual != EstadoAgenteExistencia.Peligro;

            case EstadoAgenteExistencia.Peligro:
                return true; // Este estado tiene la máxima prioridad
                
            default:
                return true;
        }
    }

    public void FinalizarEstadoActual()
    {
        if(isDebug) Debug.Log("Finaliza: " + estadoActual.ToString());
        // Si hay estado anterior, regresamos al estado anterior
        if (pilaEstados.Count > 0) {
            stateHashSet.Remove(estadoActual);
            estadoActual = pilaEstados.Pop(); if(isDebug) Debug.Log("Nuevo estado: " + estadoActual);
        }
    }

    public EstadoAgenteExistencia ObtenerEstadoActual()
    {
        return estadoActual;
    }
}

// Clase principal del agente
public class AgentePushdownAutomata : MonoBehaviour
{
    public ControladorEstados controladorEstados;
    protected EstadoAgenteExistencia estadoActual;
    public bool isDebug = false;

    [Tooltip("Variable que maneja los tiempos de la percepción interna sobre los estados: Hambre, Sed, Somnolencia")]
    public int timeMultiplier = 1;

    [System.NonSerialized]
    public string[] strEnumExistencia = System.Enum.GetNames(typeof(EstadoAgenteExistencia));

    [System.NonSerialized] public bool isAlerta = false;

    protected virtual void Start()
    {
        controladorEstados = new ControladorEstados(isDebug);
        estadoActual = controladorEstados.ObtenerEstadoActual();

        InvokeRepeating("UpdatePerception", 0f, 0.25f);
        
        //Iniciar percepción interna
        InternalPerception();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre))
                TomarDecisiones(Hambre.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Sed))
                TomarDecisiones(Sed.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Somnolencia))
                TomarDecisiones(Somnolencia.transform.position);
        }*/
        
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            //controladorEstados.FinalizarEstadoActual();
        }*/
    }
   
    void TomarDecisiones()
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinNecesidad:
                Print("El agente no tiene necesidades específicas en este momento.");
                break;
            case EstadoAgenteExistencia.Hambre:
                Print("El agente tiene hambre.");
                break;
            case EstadoAgenteExistencia.Sed:
                Print("El agente tiene sed.");
                break;
            case EstadoAgenteExistencia.Somnolencia:
                Print("El agente tiene sueño.");
                break;
            case EstadoAgenteExistencia.Amenaza:
                Print("El agente detecta amenaza.");
                break;
            case EstadoAgenteExistencia.Peligro:
                Print("¡El agente detecta peligro!");
                break;
        }
    }

    //PERCEPCION INTERNA
    void InternalPerception()
    {
        InvokeRepeating("InputSed", 8f*timeMultiplier, 15f*timeMultiplier);
        InvokeRepeating("InputHambre", 16f*timeMultiplier, 15f*timeMultiplier);
        InvokeRepeating("InputSomnolencia", 30f*timeMultiplier, 30f*timeMultiplier);
    }
    protected virtual void InputSed()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Sed))
                TomarDecisiones();
    }
    protected virtual void InputHambre()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre))
                TomarDecisiones();
    }
    protected virtual void InputSomnolencia()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Somnolencia))
                TomarDecisiones();
    }


    //PERCEPCION EXTERNA
    public float perceptionRadius = 5f;
    float coneThreshold = 1f;
    public bool debugPerception = false;

    void OnDrawGizmosSelected()
    {
        if (debugPerception) {
            // Visualizar la esfera de percepción
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, perceptionRadius);

            // Calcular y visualizar el cono de visión
            Vector3 forward = transform.forward;
            Vector3 coneDirection1 = Quaternion.Euler(0, 45, 0) * forward;
            Vector3 coneDirection2 = Quaternion.Euler(0, -45, 0) * forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + forward * 0.85f);//perceptionRadius * (coneThreshold/6));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection1 * perceptionRadius * coneThreshold);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection2 * perceptionRadius * coneThreshold);
        }
    }
    
    protected virtual void UpdatePerception() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        foreach (Collider collider in colliders) {

            if (collider.CompareTag("Player")) {
                
                Vector3 playerDirection = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, playerDirection)*1.9f;

                if(isAlerta){ dotProduct = 2f; Invoke("AlertaOff", 10f); }

                if (dotProduct > coneThreshold) {
                    
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius)) {
                        if (hit.collider.CompareTag("Player")) {
                         
                            bool endInteraction = false;
                            switch (hit.collider.gameObject.name)
                            {
                                case string a when a.Equals(strEnumExistencia[(int)EstadoAgenteExistencia.Hambre]):
                                    if(estadoActual == EstadoAgenteExistencia.Hambre) endInteraction = true;
                                    break;
                                case string a when a.Equals(strEnumExistencia[(int)EstadoAgenteExistencia.Sed]):
                                    if(estadoActual == EstadoAgenteExistencia.Sed) endInteraction = true;
                                    break;
                                case string a when a.Equals(strEnumExistencia[(int)EstadoAgenteExistencia.Somnolencia]):
                                    if (estadoActual == EstadoAgenteExistencia.Somnolencia) endInteraction = true;
                                    break;
                                case string a when a.Equals(strEnumExistencia[(int)EstadoAgenteExistencia.Amenaza]):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){ 
                                        TomarDecisiones();
                                        endInteraction = true;
                                    }
                                    break;
                                case string a when a.Equals(strEnumExistencia[(int)EstadoAgenteExistencia.Peligro]):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){ 
                                        TomarDecisiones();
                                        endInteraction = true;
                                    }
                                    break;
                            }
                            if(endInteraction){
                                controladorEstados.FinalizarEstadoActual();
                                estadoActual = controladorEstados.ObtenerEstadoActual();
                            }
                        }
                    }
                }
            }
        }
    }

    void AlertaOff(){ isAlerta = false; }
    void Print(string msg){ if(isDebug) Debug.Log(msg); }
}