using System;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System.Collections.Generic;

public partial class AgenteDeliberativoPrototipo : MonoBehaviour
{
    public string Objeto_; public Vector3 Objetivo_, ObjetivoRandom = Vector3.zero;
    public string Meta_ = Util.StrEnum(MetasAgente.SinValor);
    bool navegar = false, finalizar = false;
    public RandomPlaneSpawner rps;
    public NavMeshAgent navMeshAgent;

    void Start()
    {
        InvokeRepeating("Ir", 0f, 0.26f);//Util.frecuencia);
    }

    public void Ejecutar()
    {
        ejecutandoMeta = true;

        switch (Meta_)
        {
            case string a when a.Equals(Util.StrEnum(MetasAgente.Comer)):
                print("Comer: " + Objeto_);
                instancias[Objeto_].Remove(instancias[Objeto_].First());
                if(instancias[Objeto_].Count == 0) 
                    instancias[Util.StrEnum(EstadoAgenteRealidad.Recurso)].Remove(Objeto_);
                NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), false);
                if(Objeto_.Equals(Util.StrEnum(Objeto.Carne)))
                    NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), false);
                ejecutandoMeta = false;
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Beber)):
                print("Beber");
                instancias[Objeto_].Remove(instancias[Objeto_].First());
                if(instancias[Objeto_].Count == 0) 
                    instancias[Util.StrEnum(EstadoAgenteRealidad.Recurso)].Remove(Objeto_);
                NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Sed), false);
                ejecutandoMeta = false;
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Dormir)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Somnolencia), false);
                    print("Dormir");
                    finalizar = false;
                    ejecutandoMeta = false;
                }
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Atacar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    if(ObjetivoTemporalFinal!=null && ObjetivoTemporalFinal.name.Split("_")[0].Equals(Util.StrEnum(EstadoAgenteExistencia.Amenaza)))
                    {
                        if(instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Contains(ObjetivoTemporalFinal.name))
                            instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Remove(ObjetivoTemporalFinal.name);
                        else
                            instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Remove(instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].First());
                        ObjetivoTemporalFinal.GetComponent<DestruirAlEntrar>().toDestroy = true;
                    }
                    ObjetivoTemporal = null; ObjetivoTemporalFinal=null;
                    print("Atacar");
                    finalizar = false;
                    ejecutandoMeta = false;
                }
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Huir)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    print("Huir");
                    if(ObjetivoTemporalFinal!=null)
                    {
                        if(instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Contains(ObjetivoTemporalFinal.name))
                            instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Remove(ObjetivoTemporalFinal.name);
                        else
                            instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Remove(instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].First());
                    }else
                            instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Remove(instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].First());
                    finalizar = false;
                    ejecutandoMeta = false;
                }
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Recolectar)):
                navegar = true;
                if(finalizar)
                {
                    navegar = false;
                    if(Objetivo_ != Vector3.zero){ //Recurso_Carne_1
                        if(ObjetivoTemporalFinal.name.Split("_")[0].Equals(Util.StrEnum(EstadoAgenteRealidad.Recurso))){
                            string concepto = ObjetivoTemporalFinal.name.Split("_")[1];//Carne
                            string instancia = ObjetivoTemporalFinal.name.Split("_")[1] + ObjetivoTemporalFinal.name.Split("_")[2];//Carne_1
                            if(instancias[concepto].Count == 0) 
                                instancias[Util.StrEnum(EstadoAgenteRealidad.Recurso)].Add(concepto);
                            instancias[concepto].Add(instancia);
                            ObjetivoTemporalFinal.GetComponent<DestruirAlEntrar>().toDestroy = true;
                            print("Recolectar: " + instancia);
                        }
                        ObjetivoTemporal = null; ObjetivoTemporalFinal = null;
                    }
                    finalizar = false;
                    ejecutandoMeta = false;
                }
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Cocinar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    print("Cocinar");
                    finalizar = false;
                    //Comer
                    if((Util.StrEnum(Objeto.Carne) + Util.StrEnum(Objeto.Baya)).Contains(Objeto_)){
                        print("Comer: " + Objeto_ + " cocinada");
                        instancias[Objeto_].Remove(instancias[Objeto_].First());
                        if(instancias[Objeto_].Count == 0) 
                            instancias[Util.StrEnum(EstadoAgenteRealidad.Recurso)].Remove(Objeto_);
                        NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), false);
                        if(Objeto_.Equals(Util.StrEnum(Objeto.Carne)))
                            NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), false);
                    }//END Comer
                    ejecutandoMeta = false;
                }
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Comerciar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    print("Comerciar");
                    finalizar = false;
                    ejecutandoMeta = false;
                }
                break;
        }
        //necesidadActual = "";
        //ejecutandoMeta = false;
    }

    public void Ir()
    {
        if(Meta_.Equals(Util.StrEnum(MetasAgente.Recolectar)) && ObjetivoTemporal != null)
            if(ObjetivoTemporal.name.Contains(Util.StrEnum(EstadoAgenteRealidad.Recurso)))
                {interrumpir = true;}//Objetivo_ = ObjetivoTemporal.transform.position; ObjetivoTemporalFinal = ObjetivoTemporal;}
        if(navegar && !finalizar)
        {
            if(Objetivo_ != Vector3.zero) navMeshAgent.SetDestination(Objetivo_);
            else if(ObjetivoRandom == Vector3.zero){
                if(Meta_.Equals(Util.StrEnum(MetasAgente.Atacar)))
                {
                    finalizar = true; print("Enemigo Perdido..."); Ejecutar(); return;
                }
                
                ObjetivoRandom = rps.RandomVector();
                navMeshAgent.SetDestination(ObjetivoRandom);
                print("Explorar coordenada...");
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending){
                finalizar = true;
                ObjetivoRandom = Vector3.zero;
                Ejecutar();
            }
                
        }
    }
}

public class DataGoals
{
    public static Dictionary<string, HashSet<Tuple<string, float>>> dicGoalOntology = GetGoalElementsOntology();
    public struct Data
    {
        public string rasgo;
        public string[] etiquetas;
        public string[] prerequisitos;
        public Tuple<string, string> objetivo;//Para aquellos que utilizan instrumentacion?
    }
    public static Dictionary<string, Data> dicGoals = new Dictionary<string, Data>()
	{ { Util.StrEnum(MetasAgente.Comer), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Comida), Util.AND, Util.NOT+Util.StrEnum(EstadoAgenteBiologico.Alimentado)},
                    objetivo = Tuple.Create(Util.StrEnum(Propiedad.Comida), Util.StrEnum(Objetivo.Instantaneo)),
                    rasgo = Util.StrEnum(Rasgo.Despreocupado)} }, 
      { Util.StrEnum(MetasAgente.Beber), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Sed)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Bebida), Util.AND, Util.NOT+Util.StrEnum(EstadoAgenteBiologico.Hidratado)},
                    objetivo = Tuple.Create(Util.StrEnum(Propiedad.Bebida), Util.StrEnum(Objetivo.Instantaneo)),
                    rasgo = Util.StrEnum(Rasgo.Conservador)} },
      { Util.StrEnum(MetasAgente.Dormir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Somnolencia)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.StrEnum(EstadoAgenteBiologico.Cansado)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Introvertido)} },
      { Util.StrEnum(MetasAgente.Atacar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Amenaza), Util.StrEnum(EstadoAgenteExistencia.Peligro)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Herramienta), Util.AND, Util.StrEnum(EstadoAgenteExistencia.Amenaza)},
                    objetivo = Tuple.Create(Util.StrEnum(Objetivo.SinValor), Util.StrEnum(Objetivo.Dinamico)),
                    rasgo = Util.StrEnum(Rasgo.Egocentrico)} },
      { Util.StrEnum(MetasAgente.Huir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Amenaza), Util.StrEnum(EstadoAgenteExistencia.Peligro)},
                    prerequisitos = new string[]{Util.StrEnum(EstadoAgenteExistencia.Amenaza)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Altruista)} }, 
      { Util.StrEnum(MetasAgente.Recolectar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Sed), Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Herramienta)},
                    objetivo = Tuple.Create(Util.StrEnum(EstadoAgenteRealidad.Recurso), Util.StrEnum(Objetivo.Dinamico)),
                    rasgo = Util.StrEnum(Rasgo.Explorador)} },
      { Util.StrEnum(MetasAgente.Cocinar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Comida)},//, Util.AND, Util.StrEnum(EstadoAgenteRealidad.Agente)},//CAMBIAR
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Cocina), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Escrupuloso)} },
      { Util.StrEnum(MetasAgente.Comerciar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteRealidad.Agente), Util.StrEnum(EstadoAgenteExistencia.Sed), Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.StrEnum(EstadoAgenteRealidad.Recurso), Util.StrEnum(EstadoAgenteRealidad.Agente)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Extrovertido)}}
    };

    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()
    {
        Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = new();
        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Comer), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Baya), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Carne), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Beber), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Agua), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Dormir), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Lugar.Gremio), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Atacar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Manos), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Lanza), 1.0f),
            Tuple.Create(Util.StrEnum(EstadoAgenteExistencia.Amenaza), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Huir), new HashSet<Tuple<string, float>>(){});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Recolectar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Manos), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Azada), 1.0f),
            Tuple.Create(Util.StrEnum(EstadoAgenteRealidad.Recurso), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Cocinar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Baya), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Carne), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Comerciar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Carne), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Agua), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Baya), 1.0f)});
        
        return dicGoalElementsOntology;
    }
}