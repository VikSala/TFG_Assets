using System;
using UnityEngine;
using System.Collections.Generic;

public class AgenteDeliberativoPrototipo : BaseDeliberativo
{
    public bool compile = false;
    protected override void Awake()
    {
        base.Awake();
        
        memoria = new HashSet<string>
        {Util.StrEnum(Objeto.Manos), Util.StrEnum(Objeto.Azada), Util.StrEnum(Objeto.Agua), Util.StrEnum(Objeto.Lanza),
         Util.StrEnum(Objeto.Carne), Util.StrEnum(Lugar.Gremio), Util.StrEnum(EstadoAgenteRealidad.Recurso), 
         Util.StrEnum(Objeto.Baya), Util.StrEnum(EstadoAgenteBiologico.SinHambre), Util.StrEnum(EstadoAgenteBiologico.SinSed),
         Util.StrEnum(EstadoAgenteBiologico.Descansado), Util.StrEnum(EstadoAgenteExistencia.Amenaza)};

        instancias = new Dictionary<string, HashSet<string>>(){
        {Util.StrEnum(Objeto.Manos), new HashSet<string>{"Mis Manos"}},
        {Util.StrEnum(Objeto.Azada), new HashSet<string>{"Azada_1"}},
        {Util.StrEnum(Objeto.Lanza), new HashSet<string>{"Lanza_1"}},
        {Util.StrEnum(Objeto.Agua), new HashSet<string>{"Agua_1"}},//Lago_2//"Agua_1"
        {Util.StrEnum(Objeto.Carne), new HashSet<string>{"Carne_1"}},
        {Util.StrEnum(Objeto.Baya), new HashSet<string>{"Baya_1"}},//Huerto_1
        {Util.StrEnum(EstadoAgenteRealidad.Recurso), new HashSet<string>{"Agua", "Carne", "Baya"}},
        {Util.StrEnum(Lugar.Gremio), new HashSet<string>{"Gremio_1", "Gremio_2"}},
        {Util.StrEnum(Lugar.Cocina), new HashSet<string>{"Cocina_1"}},
        {Util.StrEnum(EstadoAgenteExistencia.Amenaza), new HashSet<string>{}}//Amenaza_Oso_1, Amenaza_Pollo_1
        };
    }

    void OnValidate()
    {
        if(compile){
            string laMeta = Util.StrEnum(MetasAgente.Comerciar);
            print("Meta " + laMeta + " tiene viabilidad: " + MetaViable(laMeta));
            compile = false;
        }
    }

    protected override void IniciarDeliberacion()
    {
        metaSelected = "";
        double result = 0; double finalResult = 0;

        if(mensajeRecibido || !ejecutandoMeta || interrumpir){
            foreach(var kv in DataGoals.dicGoals)
            {
                foreach(string etiqueta in kv.Value.etiquetas)
                {
                    if(isBreak) {isBreak = false; break;};if(interrumpir) {interrumpir = false;};

                    if(listDeseos.Count == 0 || listDeseos.Contains(etiqueta))
  
                        if(MetaViable(kv.Key)){

                            if((memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Cansado)) && etiqueta.Equals(Util.StrEnum(EstadoAgenteExistencia.Somnolencia))) ||
                                (memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Sediento)) && etiqueta.Equals(Util.StrEnum(EstadoAgenteExistencia.Sed))) ||
                                (memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Hambriento)) && etiqueta.Equals(Util.StrEnum(EstadoAgenteExistencia.Hambre)))) 
                                    result = 1;
                            else
                                if(!Meta_.Equals(kv.Key)) result += yo.Puntua(kv.Value.rasgo, memoria)/pesoPersonalidad;
                                else  result += yo.Puntua(kv.Value.rasgo, memoria)/(pesoPersonalidad*2);

                            result += GetOntologyElement(kv.Key);
                            result += ElementoDistancia(kv.Key);
                            
                            if(result > finalResult ){
                                finalResult = result;
                                metaSelected = kv.Key;
                            }
                            isBreak = true;
                        }
                    result = 0;
                }
            }
            if(metaSelected.Equals("")){listDeseos.Clear(); print("reset"); return;}
            if(ObjetivoTemporal!=null) ObjetivoTemporalFinal = ObjetivoTemporal;
            ElementoDistancia(metaSelected, true);
            GetOntologyElement(metaSelected, true);
            IniciarMeta(metaSelected);
        }
        if(!metaSelected.Equals("")) print(metaSelected + ": " + finalResult);
    }

    public override void NuevoEstado(string estado, bool masNecesidad)
    {
        string strSin, strCon, strSatisfecho = "", strNecesitado = "";
        if(estado.Equals(Util.StrEnum(EstadoAgenteExistencia.Hambre))){
            strSin = Util.StrEnum(EstadoAgenteBiologico.SinHambre);
            strCon = Util.StrEnum(EstadoAgenteBiologico.ConHambre);
            strSatisfecho = Util.StrEnum(EstadoAgenteBiologico.Alimentado);
            strNecesitado = Util.StrEnum(EstadoAgenteBiologico.Hambriento);
        }else if(estado.Equals(Util.StrEnum(EstadoAgenteExistencia.Sed))){
            strSin = Util.StrEnum(EstadoAgenteBiologico.SinSed);
            strCon = Util.StrEnum(EstadoAgenteBiologico.ConSed);
            strSatisfecho = Util.StrEnum(EstadoAgenteBiologico.Hidratado);
            strNecesitado = Util.StrEnum(EstadoAgenteBiologico.Sediento);
        }else{
            strSin = Util.StrEnum(EstadoAgenteBiologico.Descansado);
            strCon = Util.StrEnum(EstadoAgenteBiologico.Cansado);
        }

        if(masNecesidad){
            if(memoria.Contains(strSin)){
                memoria.Remove(strSin);
                memoria.Add(strCon);        //print(strCon);
            }
            else if(memoria.Contains(strSatisfecho)){
                memoria.Remove(strSatisfecho);
                memoria.Add(strCon);        //print(strCon);
            }else{
                memoria.Remove(strCon);
                memoria.Add(strNecesitado); //print(strNecesitado);
            }
        }else{
            if(memoria.Contains(strCon)){
                memoria.Remove(strCon);
                memoria.Add(strSin);        //print(strSin);
            }
            else if(memoria.Contains(strNecesitado)){
                memoria.Remove(strNecesitado);
                memoria.Add(strCon);        //print(strCon);
            }else{
                memoria.Remove(strSin);
                memoria.Add(strSatisfecho); //print(strSatisfecho);
            }
        }

        if(!strNecesitado.Equals("")) newMessage(strNecesitado);
    }

    protected override float ElementoDistancia(string meta, bool isFinal = false)
    {
        if(DataGoals.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Instantaneo))) 
            return 1;
        else if(DataGoals.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Dinamico))) 
            if(ObjetivoTemporal == null)
            {
                if(isFinal) vectorObjetivo = Vector3.zero;
                return 0.5f;
            } 
            else{
                foreach(Tuple<string, float> element in DataGoals.dicGoalOntology[meta])
                    if(ObjetivoTemporal.name.Contains(element.Item1)) 
                        if(!isFinal) return 1;
                        else vectorObjetivo = ObjetivoTemporalFinal.transform.position;
               
                return 0.5f;
            }

        float result, distancia, distanciaFinal = 0, radio = lugarManager.radioPlano;
        float umbral = radio;
        bool descansado = memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Descansado));         //+-10%
        bool trabajador = yo.myPersAttributes.Contains(Util.StrEnum(Rasgo.Escrupuloso) + yo._Total); //+-10%

        umbral = descansado ? umbral*1.1f : umbral*0.9f;
        umbral = trabajador ? umbral*1.1f : umbral*0.9f;

        //Calcular Objetivos Estáticos
        Vector3 lugar = Vector3.zero, lugarCercano = Vector3.zero;
        foreach(string instancia in instancias[DataGoals.dicGoals[meta].objetivo.Item1]){
            lugar = lugarManager.ObtenerPosicionLugar(instancia);
            distancia = Vector3.Distance(transform.position, lugar);
            if(distancia < distanciaFinal || distanciaFinal==0) 
            {
                distanciaFinal = distancia;
                lugarCercano = lugar;
            }
        }
        
        if(isFinal) vectorObjetivo = lugarCercano;

        if(distanciaFinal < umbral && distanciaFinal < radio) result = 1f;//print("El objetivo a " + distanciaFinal + " metros está cerca.");
        else if(distanciaFinal > radio && distanciaFinal > umbral) result = 0.5f;//print("El objetivo a " + distanciaFinal + " metros está lejos.");
        else result = 0f;//print("El objetivo a " + distanciaFinal + " metros está céntrico.");

        return result;
    }

    protected override void Ir()
    {
        if(Meta_.Equals(Util.StrEnum(MetasAgente.Recolectar)) && ObjetivoTemporal != null)
            if(ObjetivoTemporal.name.Contains(Util.StrEnum(EstadoAgenteRealidad.Recurso)) || ObjetivoTemporal.name.Contains(Util.StrEnum(EstadoAgenteExistencia.Amenaza)))
                {interrumpir = true;}
        if(navegar && !finalizar)
        {
            if(Objetivo_ != Vector3.zero) navMeshAgent.SetDestination(Objetivo_);
            else if(ObjetivoRandom == Vector3.zero){
                if(Meta_.Equals(Util.StrEnum(MetasAgente.Atacar)))
                {
                    finalizar = true; print("Enemigo Perdido..."); Ejecutar(); instancias[Util.StrEnum(EstadoAgenteExistencia.Amenaza)].Clear(); return;
                }
                
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
