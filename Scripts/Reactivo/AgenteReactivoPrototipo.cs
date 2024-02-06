using UnityEngine.AI;
using UnityEngine;

public class AgenteReactivoPrototipo : AgentePushdownAutomata
{
    public NavMeshAgent navMeshAgent;
    EstadoAgenteExistencia estadoAnterior;
    public GameObject Hambre, Sed, Somnolencia;
    public RandomPlaneSpawner rps;
    bool desactivarAmenaza = false;

    protected override void Start()
    {
        base.Start();
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre))
                TomarDecisiones(Hambre.transform.position);
    }

    void TomarDecisiones(Vector3 targetPosition)
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        estadoAnterior = estadoActual;
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case EstadoAgenteExistencia.Hambre:
                targetPosition = Hambre.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene hambre.", isDebug);
                break;
            case EstadoAgenteExistencia.Sed:
                targetPosition = Sed.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene sed.", isDebug);
                break;
            case EstadoAgenteExistencia.Somnolencia:
                targetPosition = Somnolencia.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene sueño.", isDebug);
                break;
            case EstadoAgenteExistencia.Amenaza:
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente detecta amenaza.", isDebug);
                break;
            case EstadoAgenteExistencia.Peligro:
                navMeshAgent.SetDestination(targetPosition); Util.Print("¡El agente detecta peligro!", isDebug);
                break;
        }
    }

    //PERCEPCION INTERNA
    protected override void InputSed()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Sed))
                TomarDecisiones(Sed.transform.position);
    }
    protected override void InputHambre()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre))
                TomarDecisiones(Hambre.transform.position);
    }
    protected override void InputSomnolencia()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Somnolencia))
                TomarDecisiones(Somnolencia.transform.position);
    }
    
    //PERCEPCION EXTERNA
    protected override void PercepcionExterna() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        if(estadoActual != estadoAnterior && (int)estadoActual < (int)EstadoAgenteExistencia.Amenaza)
            TomarDecisiones(new Vector3());

        foreach (Collider collider in colliders) {

            if (collider.CompareTag("Player")) {
                
                Vector3 playerDirection = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, playerDirection)*1.9f;

                if(isAlerta){ dotProduct = 2f; Invoke("AlertaOff", 10f); }

                if (dotProduct > 1f) {
                    
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
                                    if (estadoActual == EstadoAgenteExistencia.Somnolencia){
                                        endInteraction = true;
                                        rps.doSpawn = true;
                                    }
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Amenaza)):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza)){ 
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, 0.85f)){
                                        if(hit.collider.gameObject.name.Equals(Util.StrEnum(EstadoAgenteExistencia.Amenaza))) Destroy(hit.collider.gameObject);
                                        desactivarAmenaza = false;
                                        endInteraction = true;
                                    }   
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Peligro)):
                                    isAlerta = true;
                                    desactivarAmenaza = true;
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Peligro)){ 
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza && Physics.Raycast(transform.position, playerDirection, out hit, 0.85f)){
                                        if(hit.collider.gameObject.name.Equals(Util.StrEnum(EstadoAgenteExistencia.Peligro))) Destroy(hit.collider.gameObject);
                                        desactivarAmenaza = false;
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

}
