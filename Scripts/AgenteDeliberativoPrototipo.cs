using System;
using UnityEngine;
using System.Collections.Generic;

public class AgenteDeliberativoPrototipo : MonoBehaviour
{
    List<string> listDeseos = new List<string>{Util.StrEnum(EstadoAgenteExistencia.Hambre), Util.StrEnum(EstadoAgenteExistencia.Sed)};
    //string metaActual = Util.StrEnum(MetasAgente.SinValor);//Util.StrEnum(MetasAgente.Comerciar);
    public bool compile = false;

    //Conocimiento universal
    //Dictionary<string, DataGoals.Data> dicGoals = DataGoals.dicGoals;
    //Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = DataGoals.dicGoalOntology;

    //Memoria
    List<string> memoria = new List<string>
        {Util.StrEnum(Objeto.Manos), Util.StrEnum(Objeto.Azada), Util.StrEnum(Objeto.Agua),
         Util.StrEnum(EstadoAgenteRealidad.Recurso), Util.StrEnum(Lugar.Gremio)};//, Util.StrEnum(Objeto.Carne), Util.StrEnum(Objeto.Baya) )};


    /*              Deliberacion
	- Distancia
	- Ontologia
	- Personalidad: Hecho
	Viabilidad inicial: Prerequisitos Propiedad
	*/

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnValidate()
    {
        if(compile){
            TestearCaso();
            compile = false;
        }
    }

    void TestearCaso()
    {
        Personalidad yo = new Personalidad();
        string msg = Util.StrEnum(EstadoAgenteRealidad.Recurso); 
        string metaSelected = "";
        double result = 0;

        //Bio
        
        if(newMessage(msg)){
            foreach(var kv in DataGoals.dicGoals)
            {
                foreach(string etiqueta in kv.Value.etiquetas)
                {
                    if(msg.Equals(etiqueta))
                        if(MetaViable(kv.Key)){
                            print("Meta viable: "+ kv.Key);
                            if(yo.Puntua(kv.Value.rasgo) > result ){
                                result = yo.Puntua(kv.Value.rasgo);
                                metaSelected = kv.Key;
                            }
                        }

                }
            }
        }
        print(metaSelected);
    }

    bool newMessage(string msg)
    {
        return !listDeseos.Contains(msg);
    }

    bool MetaViable(string meta)
    {
        foreach(string requisito in DataGoals.dicGoals[meta].prerequisitos)
        {
            foreach(string cosa in memoria)
            {
                if(Util.objetoPropiedad[cosa].Equals(requisito)) return true;
            }
        }

        return false;
    }
}

public class DataGoals
{
    public static Dictionary<string, HashSet<Tuple<string, float>>> dicGoalOntology = GetGoalElementsOntology();
    public struct Data
    {
        public string[] etiquetas;
        public string[] prerequisitos;
        public string rasgo;
    }
    public static Dictionary<string, Data> dicGoals = new Dictionary<string, Data>()
	{ { Util.StrEnum(MetasAgente.Comer), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Hambre), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Comida)},
                    rasgo = Util.StrEnum(Rasgo.Despreocupado)} }, 
      { Util.StrEnum(MetasAgente.Beber), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Sed), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Bebida)},
                    rasgo = Util.StrEnum(Rasgo.Conservador)} },
      { Util.StrEnum(MetasAgente.Dormir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Somnolencia)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio)},
                    rasgo = Util.StrEnum(Rasgo.Introvertido)} },
      { Util.StrEnum(MetasAgente.Atacar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Amenaza), Util.StrEnum(EstadoAgenteExistencia.Peligro)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Herramienta)},
                    rasgo = Util.StrEnum(Rasgo.Egocentrico)} },
      { Util.StrEnum(MetasAgente.Huir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Amenaza), Util.StrEnum(EstadoAgenteExistencia.Peligro)},
                    prerequisitos = new string[]{},
                    rasgo = Util.StrEnum(Rasgo.Altruista)} }, 
      { Util.StrEnum(MetasAgente.Recolectar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Sed), Util.StrEnum(EstadoAgenteExistencia.Hambre), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Herramienta)},
                    rasgo = Util.StrEnum(Rasgo.Explorador)} },
      { Util.StrEnum(MetasAgente.Cocinar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteExistencia.Hambre), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    prerequisitos = new string[]{Util.StrEnum(Propiedad.Comida)},
                    rasgo = Util.StrEnum(Rasgo.Escrupuloso)} },
      { Util.StrEnum(MetasAgente.Comerciar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(EstadoAgenteRealidad.Agente), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.StrEnum(EstadoAgenteRealidad.Recurso)},
                    rasgo = Util.StrEnum(Rasgo.Extrovertido)}}
    };

    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()
    {
        Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = new();
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
    string inputPuntua = "Suma,Resta,Resta,Suma,Personalidad";

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
                    myPersAttributes += "Nada_" + persAttributes[x] + ",";
                    Console.WriteLine("Nada_" + persAttributes[x]);
                    break;
                case double a when a < 0.4:
                    myPersAttributes += "Poco_" + persAttributes[x] + ",";
                    Console.WriteLine("Poco_" + persAttributes[x]);
                    break;
                case double a when a < 0.6:
                    myPersAttributes += "" + persAttributes[x] + ",";
                    Console.WriteLine("" + persAttributes[x]);
                    break;
                case double a when a < 0.81:
                    myPersAttributes += "Muy_" + persAttributes[x] + ",";
                    Console.WriteLine("Muy_" + persAttributes[x]);
                    break;
                default:
                    myPersAttributes += persAttributes[x] + "_Total" + ",";
                    Console.WriteLine(persAttributes[x] + "_Total");
                    break;
            }
        }
    }

    public double Puntua(string persAtributo)
    {
        double result = 0;
        
        foreach (string attribute in myPersAttributes.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (attribute.Contains(persAtributo))
            {
                switch (attribute)
                {
                    case string a when a.Contains("Nada_"):
                        result -= 0.2;
                        break;
                    case string a when a.Contains("Poco_"):
                        result += 0.3;
                        break;
                    case string a when a.Contains("Muy_"):
                        result += 0.75;
                        break;
                    case string a when a.Contains("_Total"):
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
