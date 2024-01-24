using System.Collections.Generic;
using UnityEngine.AI;
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

    public ControladorEstados()
    {
        estadoActual = EstadoAgenteExistencia.SinNecesidad;
    }

    public bool CambiarEstado(EstadoAgenteExistencia nuevoEstado)
    {
        // Verificar prioridades antes de cambiar el estado
        if (!stateHashSet.Contains(nuevoEstado) && VerificarPrioridad(nuevoEstado))
        {
            pilaEstados.Push(estadoActual);
            estadoActual = nuevoEstado;
            stateHashSet.Add(estadoActual);
            return true;
        }
        return false;
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
        Debug.Log("Finaliza: " + estadoActual.ToString());
        // Si hay estado anterior, regresamos al estado anterior
        if (pilaEstados.Count > 0) {
            stateHashSet.Remove(estadoActual);
            estadoActual = pilaEstados.Pop(); Debug.Log("Nuevo estado: " + estadoActual);
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
    public NavMeshAgent navMeshAgent;
    private ControladorEstados controladorEstados;
    EstadoAgenteExistencia estadoActual, estadoAnterior;
    public GameObject Hambre, Sed, Somnolencia;
    bool desactivarAmenaza = false;

    private void Start()
    {
        controladorEstados = new ControladorEstados();
        estadoActual = controladorEstados.ObtenerEstadoActual();

        InvokeRepeating("UpdatePerception", 0f, 0.25f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            controladorEstados.FinalizarEstadoActual();
        }
    }

    private void TomarDecisiones(Vector3 targetPosition)
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        estadoAnterior = estadoActual;
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinNecesidad:
                //Debug.Log("El agente no tiene necesidades específicas en este momento.");
                break;
            case EstadoAgenteExistencia.Hambre:
                targetPosition = Hambre.transform.position;
                navMeshAgent.SetDestination(targetPosition); //Debug.Log("El agente tiene hambre.");
                break;
            case EstadoAgenteExistencia.Sed:
                targetPosition = Sed.transform.position;
                navMeshAgent.SetDestination(targetPosition); //Debug.Log("El agente tiene sed.");
                break;
            case EstadoAgenteExistencia.Somnolencia:
                targetPosition = Somnolencia.transform.position;
                navMeshAgent.SetDestination(targetPosition); //Debug.Log("El agente tiene sueño.");
                break;
            case EstadoAgenteExistencia.Amenaza:
                //if(isDebug) navMeshAgent.SetDestination(targetPosition);//Debug.Log("El agente detecta amenaza.");
                navMeshAgent.SetDestination(targetPosition); //Debug.Log("El agente detecta amenaza.");
                break;
            case EstadoAgenteExistencia.Peligro:
                //if(isDebug) navMeshAgent.SetDestination(targetPosition); Debug.Log("¡El agente detecta peligro!");
                navMeshAgent.SetDestination(targetPosition); //Debug.Log("¡El agente detecta peligro!");
                break;
        }
    }

    //PERCEPCION

    public float perceptionRadius = 5f;
    float coneThreshold = 1f;
    public bool debugPerception = false;
    bool isAlerta = false;
    public RandomTestSpawner rts;

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
            Gizmos.DrawLine(transform.position, transform.position + forward * perceptionRadius * (coneThreshold/6));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection1 * perceptionRadius * coneThreshold);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + coneDirection2 * perceptionRadius * coneThreshold);
        }
    }
    
    void UpdatePerception() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        if(estadoActual != estadoAnterior && estadoActual != EstadoAgenteExistencia.Amenaza)
            TomarDecisiones(new Vector3());

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
                                case "Hambre":
                                    if ((estadoActual == EstadoAgenteExistencia.Hambre) && 
                                    Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6)))
                                        endInteraction = true;
                                    break;
                                case "Sed":
                                    if ((estadoActual == EstadoAgenteExistencia.Sed) && 
                                    Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6)))
                                        endInteraction = true;
                                    break;
                                case "Somnolencia":
                                    if (estadoActual == EstadoAgenteExistencia.Somnolencia)
                                        endInteraction = true;
                                        rts.doSpawn = true;
                                    break;
                                case string a when a.Contains("Amenaza"):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6))){
                                        hit.collider.gameObject.SetActive(false);
                                        desactivarAmenaza = false;
                                        endInteraction = true;
                                    }   
                                    break;
                                case "Peligro":
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Peligro)){
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6))){
                                        hit.collider.gameObject.SetActive(false);
                                        desactivarAmenaza = false;
                                        endInteraction = true;
                                    }   
                                    break;
                            }
                            if(endInteraction){
                                controladorEstados.FinalizarEstadoActual();
                                estadoActual = controladorEstados.ObtenerEstadoActual();
                            }
                            
                            /*if (hit.collider.gameObject.name.Equals("Hambre") || hit.collider.gameObject.name.Equals("Sed")){
                                if ((estadoActual == EstadoAgenteExistencia.Hambre || estadoActual == EstadoAgenteExistencia.Sed)
                                    && Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6))){
                                        controladorEstados.FinalizarEstadoActual();
                                        estadoActual = controladorEstados.ObtenerEstadoActual();
                                }
                            }
                            else if ((estadoActual == EstadoAgenteExistencia.Somnolencia) && hit.collider.gameObject.name.Equals("Somnolencia")){
                                controladorEstados.FinalizarEstadoActual();
                                rts.doSpawn = true;
                                estadoActual = controladorEstados.ObtenerEstadoActual();
                            }
                            else if (hit.collider.gameObject.name.Equals("Peligro")){
                                isAlerta = true;
                                if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Peligro)){
                                    TomarDecisiones(hit.collider.gameObject.transform.position);
                                    desactivarAmenaza = true;
                                }
                                if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6))){//
                                        hit.collider.gameObject.SetActive(false);
                                        controladorEstados.FinalizarEstadoActual();
                                        desactivarAmenaza = false;
                                        estadoActual = controladorEstados.ObtenerEstadoActual();
                                }
                            }
                            else if (hit.collider.gameObject.name.Contains("Amenaza")){
                                isAlerta = true;
                                if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){
                                    TomarDecisiones(hit.collider.gameObject.transform.position);
                                    desactivarAmenaza = true;
                                }
                                if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius * (coneThreshold/6))){//
                                        hit.collider.gameObject.SetActive(false);
                                        controladorEstados.FinalizarEstadoActual();
                                        desactivarAmenaza = false;
                                        estadoActual = controladorEstados.ObtenerEstadoActual();
                                }
                            }*/
                        }
                    }
                }
            }
        }
    }

    void AlertaOff(){ isAlerta = false; }
}