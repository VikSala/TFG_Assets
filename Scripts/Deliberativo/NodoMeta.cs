using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public partial class BaseDeliberativo : MonoBehaviour
{
    public string Objeto_; public Vector3 Objetivo_, ObjetivoRandom = Vector3.zero;
    public string Meta_ = Util.StrEnum(Meta.SinValor);
    protected bool navegar = false, finalizar = false;
    public RandomPlaneSpawner rps;
    public NavMeshAgent navMeshAgent;


    protected virtual void Ejecutar()
    {
        ejecutandoMeta = true;

        switch (Meta_)
        {
            case string a when a.Equals(Util.StrEnum(Meta.Comer)):
                Util.Print("Comer", isDebug);
                if(instancias[Objeto_].Count != 0) instancias[Objeto_].Remove(instancias[Objeto_].First());
                if(instancias[Objeto_].Count == 0) 
                    instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Objeto_);
                NuevoEstado(Util.StrEnum(Percepcion.Hambre), false);
                if(Objeto_.Equals(Util.StrEnum(Objeto.Carne)))
                    NuevoEstado(Util.StrEnum(Percepcion.Hambre), false);
                ejecutandoMeta = false;
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Beber)):
                Util.Print("Beber", isDebug);
                instancias[Objeto_].Remove(instancias[Objeto_].First());
                if(instancias[Objeto_].Count == 0) 
                    instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Objeto_);
                NuevoEstado(Util.StrEnum(Percepcion.Sed), false);
                ejecutandoMeta = false;
                break;
        }
        
    }

    protected virtual void Ir()
    {
        if(navegar && !finalizar)
        {
            if(Objetivo_ != Vector3.zero) navMeshAgent.SetDestination(Objetivo_);
            else if(ObjetivoRandom == Vector3.zero){
                ObjetivoRandom = rps.RandomVector();
                navMeshAgent.SetDestination(ObjetivoRandom);
                //print("Explorar coordenada...");
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending){
                finalizar = true;
                ObjetivoRandom = Vector3.zero;
                Ejecutar();
            }
                
        }
    }
}