using System.Linq;
using UnityEngine;

public class AgenteReactivoFinal : AgentePushdownAutomata
{
    public GameObject AgenteDeliberativo;
    EstadoAgenteExistencia estadoAnterior;

    protected override void Start()
    {
        base.Start();
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre)){
            //controladorEstados.FinalizarEstadoActual();
            //estadoActual = controladorEstados.ObtenerEstadoActual();
            TomarDecisiones(null);
        }
    }

    void TomarDecisiones(GameObject target)
    {
        estadoActual = controladorEstados.ObtenerEstadoActual();
        if(target!=null)
        {
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().ObjetivoTemporal = target;
            
            string instancia = target.name.Split("_")[0];
            if(Util.strEnumLugares.Contains(instancia) || instancia.Equals(Util.StrEnum(EstadoAgenteExistencia.Amenaza)))
                if(!AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Contains(target.name))
                    AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Add(target.name);
        }
        estadoAnterior = estadoActual;
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case EstadoAgenteExistencia.Hambre:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Hambre));//target = Hambre.transform.position;
                //Util.Print("Reactivo detecta hambre.", true);//navMeshAgent.SetDestination(target); Util.Print("El agente tiene hambre.", isDebug);
                break;
            case EstadoAgenteExistencia.Sed:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Sed));
                //Util.Print("Reactivo detecta sed.", true);
                break;
            case EstadoAgenteExistencia.Somnolencia:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Somnolencia));
                //Util.Print("Reactivo detecta Somnolencia.", true);
                break;
            case EstadoAgenteExistencia.Amenaza:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Amenaza));
                //Util.Print("Reactivo detecta Amenaza.", true);
                break;
            case EstadoAgenteExistencia.Peligro:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Peligro));
                //Util.Print("Reactivo detecta Peligro.", true);
                break;
        }
    }

    //PERCEPCION INTERNA
    protected override void InputSed()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Sed)){
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Sed));
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Sed), true);
        }
    }
    protected override void InputHambre()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre)){
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Hambre));
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), true);
        }
    }
    protected override void InputSomnolencia()
    {
        if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Somnolencia)){
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(EstadoAgenteExistencia.Somnolencia));
            AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Somnolencia), true);
        }
    }
    
    //PERCEPCION EXTERNA
    protected override void PercepcionExterna() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        if(estadoActual != estadoAnterior && (int)estadoActual < (int)EstadoAgenteExistencia.Amenaza)
           TomarDecisiones(null);

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
                                /*case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Hambre)):
                                    if(estadoActual == EstadoAgenteExistencia.Hambre) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Sed)):
                                    if(estadoActual == EstadoAgenteExistencia.Sed) endInteraction = true;
                                    break;
                                case string a when a.Equals(Util.StrEnum(EstadoAgenteExistencia.Somnolencia)):
                                    if (estadoActual == EstadoAgenteExistencia.Somnolencia) endInteraction = true;
                                    break;*/
                                case string a when a.Contains(Util.StrEnum(EstadoAgenteExistencia.Amenaza)):
                                    if(controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza))
                                        TomarDecisiones(hit.collider.gameObject);  
                                    isAlerta = true;
                                    endInteraction = true;
                                    break;
                                case string a when a.Contains(Util.StrEnum(EstadoAgenteRealidad.Recurso)):
                                    AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().ObjetivoTemporal = hit.collider.gameObject;
                                    endInteraction = true;
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
