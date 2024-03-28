using UnityEngine.AI;
using UnityEngine;

public class AgenteReactivoPrototipo : AgentePushdownAutomata
{
    public NavMeshAgent navMeshAgent;
    Percepcion estadoAnterior;
    public GameObject Hambre, Sed, Somnolencia;
    public RandomPlaneSpawner rps;
    bool desactivarAmenaza = false;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("PercepcionExterna", 0f, 0.25f);
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(Percepcion.Hambre))
                TomarDecisiones(Hambre.transform.position);
    }

    void TomarDecisiones(Vector3 targetPosition)
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        estadoAnterior = estadoActual;
        
        switch (estadoActual)
        {
            case Percepcion.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case Percepcion.Hambre:
                targetPosition = Hambre.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene hambre.", isDebug);
                break;
            case Percepcion.Sed:
                targetPosition = Sed.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene sed.", isDebug);
                break;
            case Percepcion.Somnolencia:
                targetPosition = Somnolencia.transform.position;
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente tiene sueño.", isDebug);
                break;
            case Percepcion.Amenaza:
                navMeshAgent.SetDestination(targetPosition); Util.Print("El agente detecta amenaza.", isDebug);
                break;
            case Percepcion.Peligro:
                navMeshAgent.SetDestination(targetPosition); Util.Print("¡El agente detecta peligro!", isDebug);
                break;
        }
    }

    //PERCEPCION INTERNA
    protected override void InputSed()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Sed))
                TomarDecisiones(Sed.transform.position);
    }
    protected override void InputHambre()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Hambre))
                TomarDecisiones(Hambre.transform.position);
    }
    protected override void InputSomnolencia()
    {
        if(controladorEstados.CambiarEstado(Percepcion.Somnolencia))
                TomarDecisiones(Somnolencia.transform.position);
    }
    
    //PERCEPCION EXTERNA
    protected override void PercepcionExterna() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        if(estadoActual != estadoAnterior && (int)estadoActual < (int)Percepcion.Amenaza)//estadoActual != Percepcion.Amenaza)//
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
                                case string a when a.Equals(Util.StrEnum(Percepcion.Hambre)):
                                    if(estadoActual == Percepcion.Hambre) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Sed)):
                                    if(estadoActual == Percepcion.Sed) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Somnolencia)):
                                    if (estadoActual == Percepcion.Somnolencia){
                                        endInteraction = true;
                                        rps.doSpawn = true;
                                    }
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Amenaza)):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(Percepcion.Amenaza)){ 
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza){
                                        if(hit.collider.gameObject.name.Equals(Util.StrEnum(Percepcion.Amenaza))) hit.collider.gameObject.GetComponent<DestruirAlEntrar>().toDestroy = true;
                                        desactivarAmenaza = false;
                                        endInteraction = true;
                                    }   
                                    break;
                                case string a when a.Equals(Util.StrEnum(Percepcion.Peligro)):
                                    isAlerta = true;
                                    if(controladorEstados.CambiarEstado(Percepcion.Peligro)){ 
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                        desactivarAmenaza = true;
                                    }
                                    if (desactivarAmenaza){
                                        if(hit.collider.gameObject.name.Equals(Util.StrEnum(Percepcion.Peligro))) hit.collider.gameObject.GetComponent<DestruirAlEntrar>().toDestroy = true;//Destroy(hit.collider.gameObject);
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
