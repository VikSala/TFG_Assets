using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AgenteDeliberativoPrototipo : MonoBehaviour
{
    HashSet<string> listDeseos = new HashSet<string>{};
    //string metaActual = Util.StrEnum(MetasAgente.SinValor);//Util.StrEnum(MetasAgente.Comerciar);
    public bool compile = false;

    [NonSerialized]
    public GameObject ObjetivoTemporal;
    Personalidad yo;    NodoMeta nodoMeta; string elemento = ""; Vector3 vectorObjetivo = Vector3.zero;
    EstadoAgenteBiologico estadoAgente = EstadoAgenteBiologico.SinValor;
    public NavMeshAgent navMeshAgent;
    public LugarManager lugarManager;

    //Conocimiento universal
    //Dictionary<string, DataGoals.Data> dicGoals = DataGoals.dicGoals;
    //Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = DataGoals.dicGoalOntology;

    //Memoria
//la estructura memoria unifica los conceptos mem. largo plazo y corto plazo:
//- corto plazo: actualizacion de estados biologicos
//- largo plazo: el resto de conceptos son read_only
//Instancias tambien lo hace lp:lugares y cp:objetos
    public HashSet<string> memoria = new HashSet<string>
        {Util.StrEnum(Objeto.Manos), Util.StrEnum(Objeto.Azada), Util.StrEnum(Objeto.Agua),
         Util.StrEnum(Objeto.Carne), Util.StrEnum(Lugar.Gremio), Util.StrEnum(EstadoAgenteRealidad.Recurso), 
         Util.StrEnum(Objeto.Baya), Util.StrEnum(EstadoAgenteBiologico.SinHambre), Util.StrEnum(EstadoAgenteBiologico.SinSed),
         Util.StrEnum(EstadoAgenteBiologico.Descansado), Util.StrEnum(EstadoAgenteExistencia.Amenaza)};//Alimentado, Hidratado, Descansado

    public Dictionary<string, HashSet<string>> instancias = new Dictionary<string, HashSet<string>>(){
        {Util.StrEnum(Objeto.Manos), new HashSet<string>{"Mis Manos"}},
        {Util.StrEnum(Objeto.Azada), new HashSet<string>{"Azada_1"}},
        {Util.StrEnum(Objeto.Agua), new HashSet<string>{"Agua_1"}},//Lago_2
        {Util.StrEnum(Objeto.Carne), new HashSet<string>{"Carne_1"}},
        {Util.StrEnum(Objeto.Baya), new HashSet<string>{"Baya_1"}},//Huerto_1
        {Util.StrEnum(EstadoAgenteRealidad.Recurso), new HashSet<string>{"Agua", "Carne", "Baya"}},
        {Util.StrEnum(Lugar.Gremio), new HashSet<string>{"Gremio_1", "Gremio_2"}},
        {Util.StrEnum(EstadoAgenteExistencia.Amenaza), new HashSet<string>{}}//Amenaza_Oso_1, Amenaza_Pollo_1
        };


    /*              Deliberacion
	Util:
    - Seed
	*/

    void Start()
    {
        yo = new Personalidad();
        
    }

    void Update()
    {
        
    }

    void OnValidate()
    {
        if(compile){
            //TestearCaso();
            compile = false;
        }
    }

    void TestearCaso()
    {
        yo = new Personalidad();//Personalidad yo = new Personalidad();
        string msg = Util.StrEnum(EstadoAgenteRealidad.Recurso); 
        string metaSelected = "";
        double result = 0; double finalResult = 0;

        //EstadoAgenteBiologico
        //estadoAgente = (EstadoAgenteBiologico)(int)UnityEngine.Random.Range(1f, 10f);
        //bool umbral = (int)estadoAgente>=(int)EstadoAgenteBiologico.Cansado ? true : false; print("Estado: "+estadoAgente + " y Umbral: " + umbral);
        
        if(newMessage(msg)){
            foreach(var kv in DataGoals.dicGoals)
            {
                foreach(string etiqueta in kv.Value.etiquetas)
                {
                    //if(msg.Equals(etiqueta))//Si no hay currentMeta ni mensaje: se comenta este if
                        if(MetaViable(kv.Key)){
                            print("Meta viable: "+ kv.Key);//
                            result += yo.Puntua(kv.Value.rasgo, memoria);
                            result += GetOntologyElement(kv.Key);
                            result += ElementoDistancia(kv.Key);
                            
                            if(result > finalResult ){
                                finalResult = result;
                                metaSelected = kv.Key;
                            }
                        }
                    result = 0;
                }
            }
            GetOntologyElement(metaSelected, true);
            IniciarMeta(metaSelected);
        }
        print(metaSelected + ": " + finalResult);
    }

    public void IniciarMeta(string meta)
    {
        nodoMeta = new NodoMeta
        {
            Meta = meta,
            Objeto = elemento,
            Objetivo = vectorObjetivo
        };
        nodoMeta.Ejecutar();
    }

    public bool newMessage(string msg)
    {
        if(!listDeseos.Contains(msg)){
            listDeseos.Add(msg);
            return true;
        }
        return false;
    }

    public void NuevoEstado(string estado, bool masNecesidad)
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
                memoria.Add(strCon);        print(strCon);
            }
            else if(memoria.Contains(strSatisfecho)){
                memoria.Remove(strSatisfecho);
                memoria.Add(strCon);        print(strCon);
            }else{
                memoria.Remove(strCon);
                memoria.Add(strNecesitado); print(strNecesitado);
            }
        }else{
            if(memoria.Contains(strCon)){
                memoria.Remove(strCon);
                memoria.Add(strSin);        print(strSin);
            }
            else if(memoria.Contains(strNecesitado)){
                memoria.Remove(strNecesitado);
                memoria.Add(strCon);        print(strCon);
            }else{
                memoria.Remove(strSin);
                memoria.Add(strSatisfecho); print(strSatisfecho);
            }
        }
    }

    bool MetaViable(string meta)
    {
        bool result = false; bool and = false;

        foreach(string requisito in DataGoals.dicGoals[meta].prerequisitos)
        {
            foreach(string cosa in memoria)
            {
                if(CheckCondition(requisito, cosa)) result = true;
                else if(and && !requisito.Equals(Util.AND)) return false;
                if(requisito.Equals(Util.AND))
                    if(!result) return false;
                    else and = true;
            }
        }

        return result;
    }

    bool CheckCondition(string requisito, string cosa)
    {
        if(requisito.Equals(Util.AND)) return false;

        bool condition = true;
        string propiedad = "";
        if(requisito.Contains(Util.NOT))
        {
            requisito = requisito.Trim(Util.NOT);
            condition = false;
        }
        if (Util.objetoPropiedad.TryGetValue(cosa, out propiedad))
            if(requisito.Equals(propiedad) && instancias[requisito].Count != 0)
            { 
                foreach(string objeto in instancias[requisito])
                    if(objeto.Contains(cosa)) return condition;
                return !condition;
            }else return !condition;
        else if (instancias[requisito].Count != 0) return condition;//.Contains(requisito)) return condition;
        else return !condition;
    }

    float GetOntologyElement(string meta, bool isFinal = false)
    {
        float result = -1f; string strElement = "";

        foreach(Tuple<string, float> element in DataGoals.dicGoalOntology[meta])
        {
            if(element.Item2 > result && instancias[element.Item1].Count != 0){//memoria.Contains(element.Item1)){//uff
                strElement = element.Item1;
                result = element.Item2;
            }
        }//print("Elemento lógico: " + strElement);
        if(isFinal) elemento = strElement;
        if(result == -1) return 1f;
        else return result;
    }

    float ElementoDistancia(string meta, bool isFinal = false)
    {
        if(DataGoals.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Instantaneo))) 
            return 1;
        else if(DataGoals.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Dinamico)) && !isFinal) 
            return 0.5f;

        float result, distancia, distanciaFinal = 0, radio = lugarManager.radioPlano;
        //float objetivo = UnityEngine.Random.Range(50f, 150f);//
        float umbral = radio;
        bool descansado = memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Descansado));         //+-10%
        bool trabajador = yo.myPersAttributes.Contains(Util.StrEnum(Rasgo.Escrupuloso) + yo._Total); //+-10%

        umbral = descansado ? umbral*1.1f : umbral*0.9f;
        umbral = trabajador ? umbral*1.1f : umbral*0.9f;

        //Calcular Objetivos Estáticos
        Vector3 lugar = Vector3.zero;
        foreach(string instancia in instancias[DataGoals.dicGoals[meta].objetivo.Item1]){
            lugar = lugarManager.ObtenerPosicionLugar(instancia);
            distancia = Vector3.Distance(transform.position, lugar);
            if(distancia < distanciaFinal || distanciaFinal==0) distanciaFinal = distancia;
        }
        
        if(isFinal){
            if(DataGoals.dicGoals[meta].objetivo.Item2.Equals(Util.StrEnum(Objetivo.Dinamico)))
                vectorObjetivo = ObjetivoTemporal.transform.position;//nodoMeta.Objetivo = ObjetivoTemporal.transform.position;
            else
                vectorObjetivo = lugar;//nodoMeta.Objetivo = lugar;
        }

        if(distanciaFinal < umbral && distanciaFinal < radio) result = 1f;//print("El objetivo a " + distanciaFinal + " metros está cerca.");
        else if(distanciaFinal > radio && distanciaFinal > umbral) result = 0.5f;//print("El objetivo a " + distanciaFinal + " metros está lejos.");
        else result = 0f;//print("El objetivo a " + distanciaFinal + " metros está céntrico.");

        return result;
    }

}

public class NodoMeta : MonoBehaviour
{
    public string Objeto; public Vector3 Objetivo;
    public string Meta = Util.StrEnum(MetasAgente.SinValor);
    public AgenteDeliberativoPrototipo agenteDeliberativo;
    bool destinoAlcanzado = false;
    public RandomPlaneSpawner rps;

    public void Ejecutar()
    {
        switch (Meta)
        {
            case string a when a.Equals(Util.StrEnum(MetasAgente.Comer)):
                print("Comer");
                agenteDeliberativo.instancias[Objeto.Split("_")[0]].Remove(Objeto);//agenteDeliberativo.memoria.Remove(Objeto);//Split("_")[0]
                agenteDeliberativo.NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Hambre), false);
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Beber)):
                print("Beber");
                agenteDeliberativo.instancias[Objeto.Split("_")[0]].Remove(Objeto);//agenteDeliberativo.memoria.Remove(Objeto);
                agenteDeliberativo.NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Sed), false);
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Dormir)):
                Ir(); print("Dormir");
                if(destinoAlcanzado) agenteDeliberativo.NuevoEstado(Util.StrEnum(EstadoAgenteExistencia.Somnolencia), false);
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Atacar)):
                Ir(); print("Atacar");
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Huir)):
                Ir(); print("Huir");
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Recolectar)):
                Ir(); print("Recolectar");
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Cocinar)):
                Ir(); print("Cocinar");
                break;
            case string a when a.Equals(Util.StrEnum(MetasAgente.Comerciar)):
                Ir(); print("Comerciar");
                break;
        }
    }

    public void Ir()
    {
        if(Objetivo != null) agenteDeliberativo.navMeshAgent.SetDestination(Objetivo);
        else {
            agenteDeliberativo.navMeshAgent.SetDestination(rps.RandomVector());
            print("Explorar coordenada...");
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
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.NOT+Util.StrEnum(EstadoAgenteBiologico.Descansado)},
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
                    objetivo = Tuple.Create(Util.StrEnum(Objetivo.SinValor), Util.StrEnum(Objetivo.Dinamico)),//Depende: Lago y Huerto
                    rasgo = Util.StrEnum(Rasgo.Explorador)} },
      { Util.StrEnum(MetasAgente.Cocinar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Comida)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Cocina), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Escrupuloso)} },
      { Util.StrEnum(MetasAgente.Comerciar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteRealidad.Agente), Util.StrEnum(EstadoAgenteExistencia.Sed), Util.StrEnum(EstadoAgenteExistencia.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.StrEnum(EstadoAgenteRealidad.Recurso)},
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
            Tuple.Create(Util.StrEnum(Objeto.Lanza), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Huir), new HashSet<Tuple<string, float>>(){});

        dicGoalElementsOntology.Add(Util.StrEnum(MetasAgente.Recolectar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Manos), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Azada), 1.0f)});

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

public class Personalidad
{
    string[] persAttributes = Enum.GetNames(typeof(Rasgo));
    double[] persValues = new double[Enum.GetNames(typeof(Rasgo)).Length];
    public string myPersAttributes = "";
    public string Nada_ = "Nada_", Poco_ = "Poco_", Muy_ = "Muy_", _Total = "_Total";

    public Personalidad()
    {
        CreatePersonality();
    }

    public void CreatePersonality()
    {
        System.Random rnd = new System.Random();

        string strValues, distribution = "";
        double values, subValues = 0, first = 0, second = 0;

        for (int x = 0; x < persValues.Length; x++)
        {
            strValues = rnd.NextDouble().ToString("0.00");
            values = double.Parse(strValues);
            if (values < 0.25) values = 0.25;//min ParValues = 0.5

            if (x % 2 == 0)
            {
                subValues = values * 2;
                distribution = rnd.NextDouble().ToString("0.00");

                first = subValues * double.Parse(distribution);

                if (first > 1) first = 1;

                persValues[x] = double.Parse(first.ToString("0.00"));
            }
            else
            {
                second = subValues - (subValues * double.Parse(distribution));

                if (second > 1) second = 1;

                persValues[x] = double.Parse(second.ToString("0.00"));
            }
            //Console.WriteLine(persValues[x]);
        }
        ParsePersonality();
        Debug.Log(myPersAttributes);
    }

    void ParsePersonality()
    {
        for (int x = 0; x < persValues.Length; x++)
        {
            switch (persValues[x])
            {
                case double a when a < 0.2:
                    myPersAttributes += Nada_ + persAttributes[x] + ",";
                    Console.WriteLine(Nada_ + persAttributes[x]);
                    break;
                case double a when a < 0.4:
                    myPersAttributes += Poco_ + persAttributes[x] + ",";
                    Console.WriteLine(Poco_ + persAttributes[x]);
                    break;
                case double a when a < 0.6:
                    myPersAttributes += "" + persAttributes[x] + ",";
                    Console.WriteLine("" + persAttributes[x]);
                    break;
                case double a when a < 0.81:
                    myPersAttributes += Muy_ + persAttributes[x] + ",";
                    Console.WriteLine(Muy_ + persAttributes[x]);
                    break;
                default:
                    myPersAttributes += persAttributes[x] + _Total + ",";
                    Console.WriteLine(persAttributes[x] + _Total);
                    break;
            }
        }
    }

    public double Puntua(string persAtributo, HashSet<string> memoria)
    {
        if(memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Cansado)) || 
           memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Sediento)) ||
           memoria.Contains(Util.StrEnum(EstadoAgenteBiologico.Hambriento))) return 1;

        double result = 0;
        
        foreach (string attribute in myPersAttributes.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (attribute.Contains(persAtributo))
            {
                switch (attribute)
                {
                    case string a when a.Contains(Nada_):
                        result -= 0.2;
                        break;
                    case string a when a.Contains(Poco_):
                        result += 0.3;
                        break;
                    case string a when a.Contains(Muy_):
                        result += 0.75;
                        break;
                    case string a when a.Contains(_Total):
                        result += 1;
                        break;
                    default:
                        result += 0.5;
                        break;
                }
            }
        }

        return result;
    }

}
