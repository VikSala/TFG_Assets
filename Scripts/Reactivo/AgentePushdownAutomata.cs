using System.Collections.Generic;
using UnityEngine;
using System;

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
        estadoActual = EstadoAgenteExistencia.SinValor;
    }

    public bool CambiarEstado(EstadoAgenteExistencia nuevoEstado)
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
    public bool isDebug = false;
    protected EstadoAgenteExistencia estadoActual;

    protected virtual void Start()
    {
        controladorEstados = new ControladorEstados(isDebug);
        estadoActual = controladorEstados.ObtenerEstadoActual();

        InvokeRepeating("PercepcionExterna", 0f, Util.frecuencia);
        
        //Iniciar percepción interna
        PercepcionInterna();
    }
   
    void TomarDecisiones()
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case EstadoAgenteExistencia.Hambre:
                Util.Print("El agente tiene hambre.", isDebug);
                break;
            case EstadoAgenteExistencia.Sed:
                Util.Print("El agente tiene sed.", isDebug);
                break;
            case EstadoAgenteExistencia.Somnolencia:
                Util.Print("El agente tiene sueño.", isDebug);
                break;
            case EstadoAgenteExistencia.Amenaza:
                Util.Print("El agente detecta amenaza.", isDebug);
                break;
            case EstadoAgenteExistencia.Peligro:
                Util.Print("¡El agente detecta peligro!", isDebug);
                break;
        }
    }

    #region PERCEPCION INTERNA
    [Tooltip("Variable que maneja los tiempos de la percepción interna sobre los estados: Hambre, Sed, Somnolencia")]
    public int timeMultiplier = 1;
    void PercepcionInterna()
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
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Hambre)):
                                    if(estadoActual == EstadoAgenteExistencia.Hambre) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Sed)):
                                    if(estadoActual == EstadoAgenteExistencia.Sed) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Somnolencia)):
                                    if (estadoActual == EstadoAgenteExistencia.Somnolencia) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Amenaza)):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){ 
                                        TomarDecisiones();
                                        endInteraction = true;
                                    }
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Peligro)):
                                        isAlerta = true;
                                        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Peligro)){ 
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
