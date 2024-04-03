using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AgenteDeliberativoPrototipo : BaseDeliberativo
{
    public bool compile = false;
    protected override void Awake()
    {
        base.Awake();
        
        memoria = new HashSet<string>
        {Util.StrEnum(Objeto.Manos), Util.StrEnum(Objeto.Hoz), Util.StrEnum(Objeto.Agua), Util.StrEnum(Objeto.Espada),
         Util.StrEnum(Objeto.Carne), Util.StrEnum(Lugar.Gremio), Util.StrEnum(Percepcion.Recurso), 
         Util.StrEnum(Objeto.Baya), Util.StrEnum(Estado.SinHambre), Util.StrEnum(Estado.SinSed),
         Util.StrEnum(Estado.Descansado), Util.StrEnum(Percepcion.Amenaza)};

        instancias = new Dictionary<string, HashSet<string>>(){
        {Util.StrEnum(Objeto.Manos), new HashSet<string>{"Mis Manos"}},
        {Util.StrEnum(Objeto.Hoz), new HashSet<string>{"Hoz_1"}},
        {Util.StrEnum(Objeto.Espada), new HashSet<string>{"Espada_1"}},
        {Util.StrEnum(Objeto.Agua), new HashSet<string>{"Agua_1"}},//Lago_2//"Agua_1"
        {Util.StrEnum(Objeto.Carne), new HashSet<string>{"Carne_1"}},
        {Util.StrEnum(Objeto.Baya), new HashSet<string>{"Baya_1"}},//Huerto_1
        {Util.StrEnum(Percepcion.Recurso), new HashSet<string>{"Agua", "Carne", "Baya"}},
        {Util.StrEnum(Lugar.Gremio), new HashSet<string>{"Gremio_1", "Gremio_2"}},
        {Util.StrEnum(Lugar.Cocina), new HashSet<string>{"Cocina_1"}},
        {Util.StrEnum(Percepcion.Amenaza), new HashSet<string>{}}//Amenaza_Oso_1, Amenaza_Pollo_1
        };
    }

    void OnValidate()
    {
        if(compile){
            string laMeta = Util.StrEnum(Meta.IrLago);
            print("Meta " + laMeta + " tiene viabilidad: " + MetaViable(laMeta));
            compile = false;
        }
    }

    protected override bool BioNecesidad(string etiqueta)
    {
        return (memoria.Contains(Util.StrEnum(Estado.Cansado)) && etiqueta.Equals(Util.StrEnum(Percepcion.Somnolencia))) ||
                                (memoria.Contains(Util.StrEnum(Estado.Sediento)) && etiqueta.Equals(Util.StrEnum(Percepcion.Sed))) ||
                                (memoria.Contains(Util.StrEnum(Estado.Hambriento)) && etiqueta.Equals(Util.StrEnum(Percepcion.Hambre)));
    }

    public override void NuevoEstado(string estado, bool masNecesidad)
    {
        string strSin, strCon, strSatisfecho = "", strNecesitado = "";
        if(estado.Equals(Util.StrEnum(Percepcion.Hambre))){
            strSin = Util.StrEnum(Estado.SinHambre);
            strCon = Util.StrEnum(Estado.ConHambre);
            strSatisfecho = Util.StrEnum(Estado.Alimentado);
            strNecesitado = Util.StrEnum(Estado.Hambriento);
        }else if(estado.Equals(Util.StrEnum(Percepcion.Sed))){
            strSin = Util.StrEnum(Estado.SinSed);
            strCon = Util.StrEnum(Estado.ConSed);
            strSatisfecho = Util.StrEnum(Estado.Hidratado);
            strNecesitado = Util.StrEnum(Estado.Sediento);
        }else{
            strSin = Util.StrEnum(Estado.Descansado);
            strCon = Util.StrEnum(Estado.Cansado);
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
    }

    protected override float ElementoDistancia(string meta, bool isFinal = false)
    {
        if(DataMeta.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Instantaneo))) 
            return 1;
        else if(DataMeta.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Dinamico))) 
            if(ObjetivoTemporal == null)
            {
                if(isFinal) vectorObjetivo = Vector3.zero;
                return 0.5f;
            } 
            else{
                foreach(Tuple<string, float> element in DataMeta.dicGoalOntology[meta])
                    if(ObjetivoTemporal.name.Contains(element.Item1)) 
                        if(!isFinal) return 1;
                        else vectorObjetivo = ObjetivoTemporalFinal.transform.position;
               
                return 0.5f;
            }

        float result, distancia, distanciaFinal = 0, radio = lugarManager.radioPlano;
        float umbral = radio;
        bool descansado = memoria.Contains(Util.StrEnum(Estado.Descansado));         //+-10%
        bool trabajador = yo.myPersAttributes.Contains(Util.StrEnum(Rasgo.Escrupuloso) + yo._Total); //+-10%

        umbral = descansado ? umbral*1.1f : umbral*0.9f;
        umbral = trabajador ? umbral*1.1f : umbral*0.9f;

        //Calcular Objetivos Estáticos
        Vector3 lugar = Vector3.zero, lugarCercano = Vector3.zero;
        foreach(string instancia in instancias[DataMeta.dicGoals[meta].objetivo.Item1]){
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
        else if(distanciaFinal > radio && distanciaFinal > umbral) result = 0f;//print("El objetivo a " + distanciaFinal + " metros está lejos.");
        else result = 0.5f;//print("El objetivo a " + distanciaFinal + " metros está céntrico.");

        return result;
    }

    protected override void Ir()
    {
        bool objetivoDetectado = false;

        if(navegar && !finalizar)
        {
            if(Objetivo_ != Vector3.zero){ navMeshAgent.SetDestination(Objetivo_); objetivoDetectado = true;}
            else if(ObjetivoRandom == Vector3.zero){
                if(Meta_.Equals(Util.StrEnum(Meta.Atacar)))
                {
                    finalizar = true; 
                    Ejecutar();
                    Util.Print("Enemigo Perdido...", isDebug);
                    instancias[Util.StrEnum(Percepcion.Amenaza)].Clear(); 
                    //IniciarDeliberacion();
                    return;
                }
                
                ObjetivoRandom = rps.RandomVector();
                navMeshAgent.SetDestination(ObjetivoRandom);
            }

            if(objetivoDetectado && ObjetivoTemporalFinal == null && DataMeta.dicGoals[Meta_].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Dinamico)))
            {
                finalizar = true; 
                Ejecutar();
                Util.Print("Objetivo Perdido...", isDebug);//Tras recolectar, el reactivo lo detecta y entra aqui
                return;
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending){
                finalizar = true;
                ObjetivoRandom = Vector3.zero;
                Ejecutar();
            }
                
        }
    }

    protected override void Ejecutar()
    {
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
                if(listDeseos.Contains(Util.StrEnum(Percepcion.Hambre))) listDeseos.Remove(Util.StrEnum(Percepcion.Hambre));
                IniciarDeliberacion();
                if(metaSelected.Equals("")) IniciarDeliberacion();
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Beber)):
                Util.Print("Beber", isDebug);
                instancias[Objeto_].Remove(instancias[Objeto_].First());
                if(instancias[Objeto_].Count == 0) 
                    instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Objeto_);
                NuevoEstado(Util.StrEnum(Percepcion.Sed), false);
                if(listDeseos.Contains(Util.StrEnum(Percepcion.Sed))) listDeseos.Remove(Util.StrEnum(Percepcion.Sed));
                IniciarDeliberacion();
                if(metaSelected.Equals("")) IniciarDeliberacion();
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Dormir)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    NuevoEstado(Util.StrEnum(Percepcion.Somnolencia), false);
                    Util.Print("Dormir", isDebug);
                    ObjetivoTemporalFinal = null;
                    instancias[Util.StrEnum(Percepcion.Amenaza)].Clear();
                    rps.SpawnFrecuencia();
                    
                    if(listDeseos.Contains(Util.StrEnum(Percepcion.Somnolencia))) listDeseos.Remove(Util.StrEnum(Percepcion.Somnolencia));
                }
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Atacar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    
                    if(ObjetivoTemporalFinal!=null && ObjetivoTemporalFinal.name.Split("_")[0].Equals(Util.StrEnum(Percepcion.Amenaza))){
                        ObjetivoTemporalFinal.GetComponent<DestruirAlEntrar>().toDestroy = true;
                        Util.Print("Atacar", isDebug);
                    }
                    instancias[Util.StrEnum(Percepcion.Amenaza)].Clear();
                    
                    if(listDeseos.Contains(Util.StrEnum(Percepcion.Amenaza))) listDeseos.Remove(Util.StrEnum(Percepcion.Amenaza));
                }
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Huir)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    instancias[Util.StrEnum(Percepcion.Amenaza)].Clear();
                    Util.Print("Huir", isDebug);
                    
                    if(listDeseos.Contains(Util.StrEnum(Percepcion.Amenaza))) listDeseos.Remove(Util.StrEnum(Percepcion.Amenaza));
                }
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Recolectar)):
                navegar = true;
                if(finalizar)
                {
                    navegar = false;
                    if(Objetivo_ != Vector3.zero && ObjetivoTemporalFinal != null){ //Recurso_Carne_1
                        if(ObjetivoTemporalFinal.name.Split("_")[0].Equals(Util.StrEnum(Percepcion.Recurso))){
                            string concepto = ObjetivoTemporalFinal.name.Split("_")[1];//Carne
                            string instancia = ObjetivoTemporalFinal.name.Split("_")[1] + ObjetivoTemporalFinal.name.Split("_")[2];//Carne_1
                            if(instancias[concepto].Count == 0) 
                                instancias[Util.StrEnum(Percepcion.Recurso)].Add(concepto);
                            instancias[concepto].Add(instancia);
                            ObjetivoTemporalFinal.GetComponent<DestruirAlEntrar>().toDestroy = true;
                            Util.Print("Recolectar: " + instancia, isDebug);
                        }
                    }
                }
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Cocinar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    Util.Print("Cocinar", isDebug);
                    
                    //Comer
                    if((Util.StrEnum(Objeto.Carne) + Util.StrEnum(Objeto.Baya)).Contains(Objeto_)){
                        Util.Print("Comer: " + Objeto_ + " cocinada", isDebug);
                        instancias[Objeto_].Remove(instancias[Objeto_].First());
                        if(instancias[Objeto_].Count == 0) 
                            instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Objeto_);
                        NuevoEstado(Util.StrEnum(Percepcion.Hambre), false);
                        if(Objeto_.Equals(Util.StrEnum(Objeto.Carne)))
                            NuevoEstado(Util.StrEnum(Percepcion.Hambre), false);
                        if(listDeseos.Contains(Util.StrEnum(Percepcion.Hambre))) listDeseos.Remove(Util.StrEnum(Percepcion.Hambre));
                    }//END Comer
                }
                break;
            case string a when a.Equals(Util.StrEnum(Meta.Comerciar)):
                navegar = true;
                if(finalizar) 
                {
                    navegar = false;
                    instancias[Objeto_].Remove(instancias[Objeto_].First());
                    if(instancias[Objeto_].Count == 0) 
                        instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Objeto_);
                    string nuevaBebida = Util.StrEnum(Objeto.Agua)+instancias[Util.StrEnum(Objeto.Agua)].Count();
                    instancias[Util.StrEnum(Objeto.Agua)].Add(nuevaBebida);
                    instancias[Util.StrEnum(Percepcion.Recurso)].Add(Util.StrEnum(Objeto.Agua));
                    Util.Print("Comerciar " + Objeto_ + " por: " + nuevaBebida, isDebug);
                    //BEBER
                    Util.Print("Beber", isDebug);
                    instancias[Util.StrEnum(Objeto.Agua)].Remove(nuevaBebida);
                    if(instancias[Util.StrEnum(Objeto.Agua)].Count == 0) 
                        instancias[Util.StrEnum(Percepcion.Recurso)].Remove(Util.StrEnum(Objeto.Agua));
                    NuevoEstado(Util.StrEnum(Percepcion.Sed), false);
                    if(listDeseos.Contains(Util.StrEnum(Percepcion.Sed))) listDeseos.Remove(Util.StrEnum(Percepcion.Sed));
                    //END BEBER
                }
                break;
        }
        if(finalizar)
        {
            finalizar = false;
            IniciarDeliberacion();
            if(metaSelected.Equals("")) IniciarDeliberacion();
        }
    } 
}
