using System.Collections.Generic;
using UnityEngine;

// Clase para manejar el estado actual y la pila de estados
public class ControladorEstados
{
    private Percepcion estadoActual;
    private Stack<Percepcion> pilaEstados = 
                    new Stack<Percepcion>();
    private HashSet<Percepcion> stateHashSet = 
                    new HashSet<Percepcion>();

    public bool isDebug = false;

    public ControladorEstados(bool isDebug)
    {
        this.isDebug = isDebug;
        estadoActual = Percepcion.SinValor;
    }

    public bool CambiarEstado(Percepcion nuevoEstado)
    {
        bool verificacion = VerificarPrioridad(nuevoEstado);
        // Verificar prioridades y duplicidad antes de cambiar el estado
        if (!stateHashSet.Contains(nuevoEstado) && verificacion)
        {
            pilaEstados.Push(estadoActual);
            estadoActual = nuevoEstado;
            stateHashSet.Add(estadoActual);
        }
        return verificacion;
    }

    private bool VerificarPrioridad(Percepcion nuevoEstado)
    {
        switch (nuevoEstado)
        {
            case Percepcion.Hambre:
                return estadoActual != Percepcion.Peligro && 
                estadoActual != Percepcion.Amenaza && 
                estadoActual != Percepcion.Somnolencia;

            case Percepcion.Sed:
                return estadoActual != Percepcion.Peligro && 
                estadoActual != Percepcion.Amenaza && 
                estadoActual != Percepcion.Somnolencia;

            case Percepcion.Somnolencia:
                return estadoActual != Percepcion.Peligro && 
                estadoActual != Percepcion.Amenaza;

            case Percepcion.Amenaza:
                return estadoActual != Percepcion.Peligro;

            case Percepcion.Peligro:
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

    public Percepcion ObtenerEstadoActual()
    {
        return estadoActual;
    }

    public bool Contiene(Percepcion estado)
    {
        return stateHashSet.Contains(estado);
    }
}

// Clase principal del agente
public class AgentePushdownAutomata : MonoBehaviour
{
    public ControladorEstados controladorEstados;
    public bool isDebug = false;
    protected Percepcion estadoActual;

    protected virtual void Start()
    {
        controladorEstados = new ControladorEstados(isDebug);
        estadoActual = controladorEstados.ObtenerEstadoActual();
        
        //Iniciar percepción interna
        PercepcionInterna();
    }
   
    void TomarDecisiones()
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        
        switch (estadoActual)
        {
            case Percepcion.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case Percepcion.Hambre:
                Util.Print("El agente tiene hambre.", isDebug);
                break;
            case Percepcion.Sed:
                Util.Print("El agente tiene sed.", isDebug);
                break;
            case Percepcion.Somnolencia:
                Util.Print("El agente tiene sueño.", isDebug);
                break;
            case Percepcion.Amenaza:
                Util.Print("El agente detecta amenaza.", isDebug);
                break;
            case Percepcion.Peligro:
                Util.Print("¡El agente detecta peligro!", isDebug);
                break;
        }
    }

    #region PERCEPCION INTERNA
    [Tooltip("Variable que maneja los tiempos de la percepción interna sobre los estados: Hambre, Sed, Somnolencia")]
    public float timeMultiplier = 1;
    protected void PercepcionInterna()
    {
        InvokeRepeating("InputHambre", 8f*timeMultiplier, 15f*timeMultiplier);
        InvokeRepeating("InputSed", 16f*timeMultiplier, 15f*timeMultiplier);
        InvokeRepeating("InputSomnolencia", 30f*timeMultiplier, 30f*timeMultiplier);
    }
    protected virtual void InputSed()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Sed))
                TomarDecisiones();
    }
    protected virtual void InputHambre()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Hambre))
                TomarDecisiones();
    }
    protected virtual void InputSomnolencia()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Somnolencia))
                TomarDecisiones();
    }
    #endregion

    #region PERCEPCION EXTERNA
    protected bool isAlerta = false;
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
            Gizmos.DrawLine(transform.position, transform.position + forward * 0.85f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection1 * perceptionRadius * coneThreshold);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection2 * perceptionRadius * coneThreshold);
        }
    }
    
    protected virtual void PercepcionExterna() {
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
                                case string a when a.Equals(Util.StrEnum(Percepcion.Hambre)):
                                    if(estadoActual == Percepcion.Hambre) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Sed)):
                                    if(estadoActual == Percepcion.Sed) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Somnolencia)):
                                    if (estadoActual == Percepcion.Somnolencia) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Amenaza)):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(Percepcion.Amenaza)){ 
                                        TomarDecisiones();
                                        endInteraction = true;
                                    }
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Peligro)):
                                        isAlerta = true;
                                        if(controladorEstados.CambiarEstado(Percepcion.Peligro)){ 
                                            TomarDecisiones();
                                            endInteraction = true;
                                        }
                                    break;
                                default:
                                    Util.Print("El agente detecta: " + hit.collider.gameObject.name, isDebug);
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

    protected void AlertaOff(){ isAlerta = false; }
    #endregion
}
