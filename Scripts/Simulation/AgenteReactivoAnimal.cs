using UnityEngine.AI;
using UnityEngine;

public class AgenteReactivoAnimal : AgentePushdownAutomata
{
    public NavMeshAgent navMeshAgent;   Vector3 posicionTemporal = Vector3.zero;
    Percepcion estadoAnterior;
    public GameObject Hambre, Sed;
    public RandomPlaneSpawner rps;
    bool estoyDurmiendo = false, estoyAsustado = false;
    float velocidadOriginal;
    string nombre;

    protected override void Start()
    {
        base.Start();
        nombre = gameObject.name;
        velocidadOriginal = navMeshAgent.speed;
        InvokeRepeating("PercepcionExterna", 0f, 0.25f);
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(Percepcion.Hambre))
                TomarDecisiones(Vector3.zero);
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
                navMeshAgent.speed = velocidadOriginal;
                Util.Print("El animal tiene hambre.", isDebug);
                if(Hambre != null) 
                {
                    targetPosition = Hambre.transform.position;
                    Ir(targetPosition); Util.Print("El animal va a comer.", isDebug);
                    
                } else Ir(rps.RandomVector());
                break;
            case Percepcion.Sed:
                navMeshAgent.speed = velocidadOriginal;
                Util.Print("El animal tiene sed.", isDebug);
                if(Sed != null) 
                {
                    targetPosition = Sed.transform.position;
                    Ir(targetPosition); Util.Print("El animal va a beber.", isDebug);
                    FinalizarEstadoActual();
                } else Ir(rps.RandomVector());
                break;
            case Percepcion.Somnolencia:
                navMeshAgent.speed = velocidadOriginal;
                Util.Print("El animal duerme.", isDebug);
                estoyDurmiendo = true;
                Invoke("Durmiendo", 5f);
                break;
            case Percepcion.Amenaza:
                //Util.Print("El agente detecta amenaza.", isDebug);
                navMeshAgent.speed = velocidadOriginal*1.25f;

                if(controladorEstados.Contiene(Percepcion.Hambre)){
                    if(controladorEstados.CambiarEstado(Percepcion.Peligro)) 
                        {Invoke("Perseguir", 5f); Util.Print("¡El agente ataca al peligro!", isDebug); TomarDecisiones(targetPosition);}
                }else 
                { 
                    if(posicionTemporal == Vector3.zero)
                    {
                        posicionTemporal = rps.RandomVector(); Util.Print("El agente detecta amenaza y huye.", isDebug);
                        Ir(posicionTemporal);
                    } 
                }
                break;
            case Percepcion.Peligro:
                
                if(!estoyAsustado) {
                    Ir(targetPosition); 
                    gameObject.transform.parent.name += "_" + Util.StrEnum(Percepcion.Amenaza); 
                }   
                break;
        }
    }

    void FinalizarEstadoActual()
    {
        if(estadoActual.Equals(Percepcion.Peligro)) gameObject.name = nombre; 
        controladorEstados.FinalizarEstadoActual();
        estadoActual = controladorEstados.ObtenerEstadoActual();
    }

    void Durmiendo(){ estoyDurmiendo = false; FinalizarEstadoActual();}
    void Perseguir(){ estoyAsustado = true; Ir(rps.RandomVector()); FinalizarEstadoActual(); }
    void Ir(Vector3 destino){ navMeshAgent.isStopped = true; navMeshAgent.SetDestination(destino); navMeshAgent.isStopped = false; }

    //PERCEPCION INTERNA
    protected override void InputSed()
    {
        if(!estoyDurmiendo && controladorEstados.CambiarEstado(Percepcion.Sed))
                if(Sed != null) TomarDecisiones(Sed.transform.position);
                else TomarDecisiones(Vector3.zero);
    }
    protected override void InputHambre()
    {
        if(!estoyDurmiendo && controladorEstados.CambiarEstado(Percepcion.Hambre))
                if(Hambre != null) TomarDecisiones(Hambre.transform.position);
                else TomarDecisiones(Vector3.zero);
    }
    protected override void InputSomnolencia()
    {
        if(!estoyDurmiendo && controladorEstados.CambiarEstado(Percepcion.Somnolencia))
                TomarDecisiones(Vector3.zero);
    }
    
    //PERCEPCION EXTERNA
    protected override void PercepcionExterna() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        if(estadoActual != estadoAnterior && (int)estadoActual < (int)Percepcion.Amenaza)
            TomarDecisiones(new Vector3());
        else if(estadoActual == Percepcion.Amenaza)
        {
            if(!isAlerta){ posicionTemporal = Vector3.zero; estoyAsustado = false; FinalizarEstadoActual(); navMeshAgent.enabled = false; navMeshAgent.enabled = true; }
        }

        foreach (Collider collider in colliders) {

            if (collider.CompareTag(Util.StrEnum(Entidad.Player))) {
                
                Vector3 playerDirection = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, playerDirection)*1.9f;

                if(isAlerta){ dotProduct = 2f; Invoke("AlertaOff", 10f); }

                if (dotProduct > 1f) {
                    
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius)) {
                        if (hit.collider.CompareTag(Util.StrEnum(Entidad.Player))) {
                            switch (hit.collider.gameObject.name)
                            {
                                case string a when a.Contains(Util.StrEnum(Objeto.Carne)) || a.Contains(Util.StrEnum(Objeto.Baya)):
                                    Hambre = hit.collider.gameObject;
                                    if(estadoActual.Equals(Percepcion.Hambre)) 
                                    {
                                        TomarDecisiones(Hambre.transform.position);
                                        hit.collider.gameObject.GetComponent<DestruirAlEntrar>().toDestroy = true;
                                        Hambre = null;
                                        FinalizarEstadoActual();
                                    }
                                    break;
                                case string a when a.Contains(Util.StrEnum(Lugar.Lago)):
                                    Sed = hit.collider.gameObject;
                                    break;
                                case string a when a.Contains(Util.StrEnum(Entidad.Agente)):
                                    isAlerta = true;
                                    if(!estoyDurmiendo && controladorEstados.CambiarEstado(Percepcion.Amenaza))
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

}
