using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class AgenteReactivoAnimal : AgentePushdownAutomata
{
    public NavMeshAgent navMeshAgent;   Vector3 posicionTemporal = Vector3.zero; //bool congelar = false;
    public GameObject ObjetoCarne;
    Percepcion estadoAnterior;
    public GameObject Hambre, Sed;
    RandomPlaneSpawner rps;
    bool estoyDurmiendo = false, estoyAsustado = false, ejecutandoEfecto = false, inicio = true;
    float velocidadOriginal;
    string nombre, id;
    [System.NonSerialized]public bool congelar = false;//cazado

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) print(estadoActual);
    }*/

    protected override void Start()
    {
        base.Start();
        rps = GameObject.FindWithTag("GameController").GetComponent<RandomPlaneSpawner>();
        GetComponent<AnimChangerLayer>().Animar("Descansar", AnimChangerLayer.Layer.Base);
        nombre = gameObject.name;
        id = gameObject.name.Split("_")[2];
        velocidadOriginal = navMeshAgent.speed;
        InvokeRepeating("PercepcionExterna", 0f, Random.Range(0f, 0.25f));
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(Percepcion.Hambre))
                TomarDecisiones(Vector3.zero);
    }

    void TomarDecisiones(Vector3 targetPosition)
    {   if(congelar) return;
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
                    
                } else {Ir(rps.RandomVector()); StartCoroutine(InvocarEfecto(Percepcion.SinValor, (float)Tiempo.Corto));}
                break;
            case Percepcion.Sed:
                navMeshAgent.speed = velocidadOriginal;
                Util.Print("El animal tiene sed.", isDebug);
                if(Sed != null) 
                {
                    targetPosition = Sed.transform.position;
                    Ir(targetPosition); Util.Print("El animal va a beber.", isDebug);
                } else Ir(rps.RandomVector());
                break;
            case Percepcion.Somnolencia:
                navMeshAgent.speed = velocidadOriginal;
                Util.Print("El animal duerme.", isDebug);
                estoyDurmiendo = true;
                GetComponent<AnimChangerLayer>().Animar("Dormir", AnimChangerLayer.Layer.Base);
                ejecutandoEfecto = true;
                gameObject.name = Util.StrEnum(Percepcion.Recurso) + "_" + id;
                navMeshAgent.isStopped = true;
                StartCoroutine(InvocarEfecto(Percepcion.Somnolencia, (float)Tiempo.Largo));//Invoke("Meta", (float)Tiempo.Largo);
                break;
            case Percepcion.Amenaza:
                navMeshAgent.speed = velocidadOriginal*1.25f;//Util.Print("El agente detecta amenaza.", isDebug);

                if(controladorEstados.Contiene(Percepcion.Hambre)){
                    if(controladorEstados.CambiarEstado(Percepcion.Peligro)) 
                        {   StartCoroutine(InvocarEfecto(Percepcion.Peligro, (float)Tiempo.Medio));//Invoke("Meta", (float)Tiempo.Medio); 
                            Util.Print("¡El agente ataca al peligro!", isDebug); TomarDecisiones(targetPosition);}
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
                    gameObject.name = Util.StrEnum(Percepcion.Amenaza) + "_" + id; 
                }   
                break;
        }
    }

    void FinalizarEstadoActual()
    { 
        controladorEstados.FinalizarEstadoActual();
        estadoActual = controladorEstados.ObtenerEstadoActual();
    }

    void Ir(Vector3 destino)
    { 
        navMeshAgent.isStopped = congelar; //if(congelar) {Morir(); return;}
        navMeshAgent.SetDestination(destino); 

        if(estadoActual == Percepcion.Peligro || estadoActual == Percepcion.Amenaza)
            GetComponent<AnimChangerLayer>().Animar("Correr", AnimChangerLayer.Layer.Base);
        else
            GetComponent<AnimChangerLayer>().Animar("Andar", AnimChangerLayer.Layer.Base);
    }

    void Morir()
    {
        Instantiate(ObjetoCarne, gameObject.transform.position, Quaternion.identity); 
        //congelar = true;
        Destroy(gameObject);
        //cazado = false;
    }
    
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
        if (congelar) {Morir(); return;}

        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);
        
        //if(navMeshAgent.isStopped && !estoyDurmiendo) Destroy(gameObject);

        if (navMeshAgent.isOnNavMesh && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            if(!ejecutandoEfecto && !inicio) GetComponent<AnimChangerLayer>().Animar("Descansar", AnimChangerLayer.Layer.Base);

        if(estadoActual != estadoAnterior && (int)estadoActual < (int)Percepcion.Amenaza)
            TomarDecisiones(new Vector3());
        else if(estadoActual == Percepcion.Amenaza)
        {
            if(!isAlerta){ posicionTemporal = Vector3.zero; estoyAsustado = false; FinalizarEstadoActual(); }
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
                                    if(estadoActual.Equals(Percepcion.Hambre) && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) 
                                    {
                                        TomarDecisiones(Hambre.transform.position);
                                        GetComponent<AnimChangerLayer>().Animar("Consumir", AnimChangerLayer.Layer.Base);  
                                        ejecutandoEfecto = true;                                      
                                        StartCoroutine(InvocarEfecto(Percepcion.Hambre, (float)Tiempo.Corto));//Invoke("Meta", (float)Tiempo.Corto);
                                    }
                                    break;
                                case string a when a.Contains(Util.StrEnum(Lugar.Lago)):
                                    Sed = hit.collider.gameObject;
                                    if(estadoActual.Equals(Percepcion.Sed) && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) 
                                    {
                                        TomarDecisiones(Sed.transform.position);
                                        GetComponent<AnimChangerLayer>().Animar("Consumir", AnimChangerLayer.Layer.Base);
                                        ejecutandoEfecto = true;
                                        StartCoroutine(InvocarEfecto(Percepcion.Sed, (float)Tiempo.Corto));//Invoke("Meta", (float)Tiempo.Corto);
                                    }
                                    break;
                                case string a when a.Contains(Util.StrEnum(Entidad.Agente))/* || a.Contains(Util.StrEnum(Entidad.Animal))*/:
                                    isAlerta = true;
                                    if(!estoyDurmiendo && controladorEstados.CambiarEstado(Percepcion.Amenaza))
                                        TomarDecisiones(hit.collider.gameObject.transform.position);
                                    if(estadoActual.Equals(Percepcion.Peligro) && !estoyAsustado && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                                        GetComponent<AnimChangerLayer>().Animar("Atacar", AnimChangerLayer.Layer.Base); 
                                    break;
                            }
                        }
                    }
                }
            }
        }

        if(estadoActual == Percepcion.Sed && Sed != null && !ejecutandoEfecto)
        {
            GetComponent<AnimChangerLayer>().Animar("Consumir", AnimChangerLayer.Layer.Base);
            ejecutandoEfecto = true;
            StartCoroutine(InvocarEfecto(Percepcion.Sed, (float)Tiempo.Corto));//Invoke("Meta", (float)Tiempo.Corto);
        }
    }
    
    IEnumerator InvocarEfecto(Percepcion estado, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        
        Meta(estado);
    }

    void Meta(Percepcion estado)
    {
        inicio = false;
        if(congelar || !estado.Equals(estadoActual)) return;
        ejecutandoEfecto = false;//GetComponent<AnimChangerLayer>().Animar("Descansar", AnimChangerLayer.Layer.Base);

        switch (estado)
        {
            case Percepcion.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case Percepcion.Hambre:
                FinalizarEstadoActual();    
                if(Hambre!=null) 
                {
                    Util.Print("El agente ha comido.", isDebug);
                    Hambre.GetComponent<DestruirAlEntrar>().toDestroy = true;
                    Hambre = null;
                }
                break;
            case Percepcion.Sed:
                FinalizarEstadoActual();    Util.Print("El agente ha bebido.", isDebug);
                break;
            case Percepcion.Somnolencia:
                estoyDurmiendo = false; FinalizarEstadoActual();    Util.Print("El agente ha dormido.", isDebug); 
                gameObject.name = nombre;
                break;
            case Percepcion.Peligro:
                estoyAsustado = true; 
                Ir(rps.RandomVector()); 
                FinalizarEstadoActual(); 
                gameObject.name = nombre; 
                Util.Print("El agente ha huido.", isDebug);
                break;
        }
    }

}