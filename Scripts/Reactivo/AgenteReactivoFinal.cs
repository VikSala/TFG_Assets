using System.Linq;
using UnityEngine;

public class AgenteReactivoFinal : AgentePushdownAutomata
{
    public GameObject AgenteDeliberativo;
    Percepcion estadoAnterior;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("PercepcionExterna", 0f, 0.25f);
        
        //Iniciar con comer
        if(controladorEstados.CambiarEstado(Percepcion.Hambre)){
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
            if(Util.strEnumLugar.Contains(instancia) || instancia.Equals(Util.StrEnum(Percepcion.Amenaza)))
                if(!AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Contains(target.name))
                    AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Add(target.name);
        }
        estadoAnterior = estadoActual;
        
        switch (estadoActual)
        {
            case Percepcion.SinValor:
                Util.Print("El agente no tiene necesidades específicas en este momento.", isDebug);
                break;
            case Percepcion.Hambre:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(Percepcion.Hambre));
                break;
            case Percepcion.Sed:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(Percepcion.Sed));
                break;
            case Percepcion.Somnolencia:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(Percepcion.Somnolencia));
                break;
            case Percepcion.Amenaza:
                AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(Percepcion.Amenaza));
                break;
        }
    }

    //PERCEPCION INTERNA
    protected override void InputSed()
    {
        AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(Percepcion.Sed), true);
        if(controladorEstados.CambiarEstado(Percepcion.Sed)) TomarDecisiones(null);
    }
    protected override void InputHambre()
    {
        AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(Percepcion.Hambre), true);
        if(controladorEstados.CambiarEstado(Percepcion.Hambre)) TomarDecisiones(null);
    }
    protected override void InputSomnolencia()
    {
        AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().NuevoEstado(Util.StrEnum(Percepcion.Somnolencia), true);
        if(controladorEstados.CambiarEstado(Percepcion.Somnolencia)) TomarDecisiones(null);
    }
    
    //PERCEPCION EXTERNA
    protected override void PercepcionExterna() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptionRadius);

        //if(estadoActual != estadoAnterior && (int)estadoActual < (int)Percepcion.Amenaza)
            //TomarDecisiones(null);

        foreach (Collider collider in colliders) {

            if (collider.CompareTag(Util.StrEnum(Entidad.Player))) {
                
                Vector3 playerDirection = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, playerDirection)*1.9f;

                if(isAlerta){ dotProduct = 2f; Invoke("AlertaOff", 10f); }

                if (dotProduct > 1f) {
                    
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, playerDirection, out hit, perceptionRadius)) {
                        if (hit.collider.CompareTag(Util.StrEnum(Entidad.Player))) {
                            
                            bool endInteraction = false;
                            switch (hit.collider.gameObject.name)
                            {
                                case string a when a.Contains(Util.StrEnum(Percepcion.Amenaza)):
                                    if(controladorEstados.CambiarEstado(Percepcion.Amenaza))
                                        TomarDecisiones(hit.collider.gameObject);  
                                    isAlerta = true;
                                    endInteraction = true;
                                    break;
                                case string a when a.Contains(Util.StrEnum(Percepcion.Recurso)):
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
