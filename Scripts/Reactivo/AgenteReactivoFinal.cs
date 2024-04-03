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
            if(instancia.Equals(Util.StrEnum(Percepcion.Amenaza)))
                if(!AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Contains(target.name))
                    AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().instancias[instancia].Add(target.name);
        }
        estadoAnterior = estadoActual;
        
        AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().newMessage(Util.StrEnum(estadoActual));
        
        controladorEstados.FinalizarEstadoActual();
        estadoActual = controladorEstados.ObtenerEstadoActual();
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
                                case string a when a.Contains(Util.StrEnum(Percepcion.Amenaza)):
                                    if(controladorEstados.CambiarEstado(Percepcion.Amenaza))
                                        TomarDecisiones(hit.collider.gameObject);  
                                    isAlerta = true;
                                    break;
                                case string a when a.Contains(Util.StrEnum(Percepcion.Recurso)):
                                    AgenteDeliberativo.GetComponent<AgenteDeliberativoPrototipo>().ObjetivoTemporal = hit.collider.gameObject;
                                    if(controladorEstados.Contiene(Percepcion.Hambre)) TomarDecisiones(hit.collider.gameObject);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    
}
