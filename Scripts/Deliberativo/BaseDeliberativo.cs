using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(DatosEntidad))]
public partial class BaseDeliberativo : MonoBehaviour
{
    protected HashSet<string> listDeseos = new HashSet<string>{};
    public bool isDebug = false;
    protected bool isBreak = false;

    [NonSerialized]
    public GameObject ObjetivoTemporal, ObjetivoTemporalFinal;
    protected GameObject Elemento_, ElementoTemporal;
    protected Personalidad yo;    string elemento = ""; protected Vector3 vectorObjetivo;
    public LugarManager lugarManager;
    protected string metaSelected = "";
    protected bool mensajeRecibido = false;
    
    [Tooltip("Un valor alto le da menos peso a la personalidad, lo que significa: un comportamiento más sólido(menos emergente o variable).")]
    [Range(0.5f, 2f)]public float pesoPersonalidad = 1;
    protected float frecuencia;

    protected string Objeto_; protected Vector3 Objetivo_, ObjetivoRandom = Vector3.zero;
    protected string Meta_ = Util.StrEnum(Meta.SinValor);
    protected bool navegar = false, finalizar = false;
    public RandomPlaneSpawner rps;
    public NavMeshAgent navMeshAgent;

    //Conocimiento universal
    //Dictionary<string, DataMeta.Data> dicGoals = DataMeta.dicGoals;
    //Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = DataMeta.dicGoalOntology;

    //Memoria
//la estructura memoria unifica los conceptos mem. largo plazo y corto plazo:
//- corto plazo: actualizacion de estados biologicos
//- largo plazo: el resto de conceptos son read_only
//Instancias tambien lo hace lp:lugares y cp:objetos
    protected HashSet<string> memoria = new HashSet<string>{};

    public Dictionary<string, HashSet<string>> instancias = new Dictionary<string, HashSet<string>>(){};

    string nombreAgente = "";

    protected virtual void Awake()
    {
        frecuencia = UnityEngine.Random.Range(0f, 0.25f);
        nombreAgente = gameObject.name;
        yo = new Personalidad();

        GetComponent<DatosEntidad>().Nombre = nombreAgente;
        GetComponent<DatosEntidad>().Frecuencia = frecuencia;
        GetComponent<DatosEntidad>().RasgosPersonalidad = new List<string>(yo.myPersAttributes.Split(",", StringSplitOptions.RemoveEmptyEntries));
    }

    void Start()//protected virtual 
    {
        //IniciarDeliberacion();//InvokeRepeating("IniciarDeliberacion", 0f, frecuencia);
        InvokeRepeating("Ir", 0f, frecuencia);
    }

    protected void IniciarDeliberacion()
    {
        metaSelected = "";
        double result, finalResult = 0;

        if(mensajeRecibido|| !finalizar){// || !ejecutandoMeta || interrumpir){
            foreach(var kv in DataMeta.dicGoals)
            {
                foreach(string etiqueta in kv.Value.etiquetas)
                {
                    //if(interrumpir) {interrumpir = false;};

                    if(listDeseos.Count == 0 || listDeseos.Contains(etiqueta)){
  
                        if(MetaViable(kv.Key)){
                            result = 0; 
                            if(BioNecesidad(etiqueta)) result = 1;
                            else result += yo.Puntua(kv.Value.rasgo)/pesoPersonalidad;//else if(!Meta_.Equals(kv.Key)) result += yo.Puntua(kv.Value.rasgo)/pesoPersonalidad;
                            //else  result += yo.Puntua(kv.Value.rasgo)/(pesoPersonalidad*2);

                            result += GetOntologyElement(kv.Key);
                            result += ElementoDistancia(kv.Key);
                            
                            if(result > finalResult ){
                                finalResult = result;
                                metaSelected = kv.Key;
                            }
                        }
                        isBreak = true;
                    }
                    
                    if(isBreak) {isBreak = false; break;};
                }
            }
            if(metaSelected.Equals("")){listDeseos.Clear();  GetComponent<DatosEntidad>().Resets++; return;}//Util.Print("Reset: " + nombreAgente, true);
            if(ObjetivoTemporal!=null) ObjetivoTemporalFinal = ObjetivoTemporal;
            ElementoDistancia(metaSelected, true);
            GetOntologyElement(metaSelected, true);
            IniciarMeta(metaSelected);
        }
        if(!metaSelected.Equals("")) Util.Print(metaSelected + ": " + finalResult, isDebug);
    }

    public void IniciarMeta(string meta)
    {
        /*foreach(string etiqueta in DataMeta.dicGoals[metaSelected].etiquetas) 
            if(listDeseos.Contains(etiqueta))
            {
                listDeseos.Remove(etiqueta);
                break;
            }*/

        Meta_ = meta;
        Objeto_ = elemento;
        Objetivo_ = vectorObjetivo;
        Elemento_ = ElementoTemporal;
        ObjetivoTemporal = null; //destinoAlcanzado = false;
        Ejecutar();
    }

    public void newMessage(string msg)
    {
        if(!listDeseos.Contains(msg)){
            listDeseos.Add(msg);
            mensajeRecibido = true;
            IniciarDeliberacion();
            if(metaSelected.Equals("")) IniciarDeliberacion();
        }
        mensajeRecibido = false;
    }

    protected virtual bool BioNecesidad(string etiqueta)
    {
        return false;
    }

    public virtual void NuevoEstado(string estado, bool masNecesidad)
    {
        string strSin, strCon, strSatisfecho = "", strNecesitado = "";

        if(estado.Equals("Hambre")){
            strSin = "SinHambre"; strCon = "ConHambre";
            strSatisfecho = "Alimentado"; strNecesitado = "Hambriento";
            print(strSin + strCon +  strSatisfecho + strNecesitado); 
        }

        if(masNecesidad){
            print("Se opera con la memoria el estado biológico.");
        }else{
            print("Se opera con la memoria el estado biológico.");
        }
    }

    protected bool MetaViable(string meta)
    {
        bool result = false; bool and = false;

        foreach(string requisito in DataMeta.dicGoals[meta].prerrequisitos)
        {
            foreach(string cosa in memoria)
            {
                if(CheckCondition(requisito, cosa)) {result = true; break;}
                else if(and && !requisito.Equals(Util.AND)) return false;
                if(requisito.Equals(Util.AND))
                    if(!result) return false;
                    else {and = true; break;}
            }
        }

        return result;
    }

    protected bool CheckCondition(string requisito, string cosa)
    {
        if(requisito.Equals(Util.AND)) return false;

        bool condition = true;
        HashSet<string> container;
        string propiedad = "";
        if(requisito.Contains(Util.NOT))
        {
            requisito = requisito.Trim(Util.NOT);
            condition = false;
        }

        if(!instancias.TryGetValue(requisito, out container)) 
            if(memoria.Contains(requisito)) return condition;
            else if(Util.tipoObjeto.TryGetValue(cosa, out propiedad) && !requisito.Equals(propiedad)) return !condition;
        
        if ((instancias.TryGetValue(requisito, out container) || Util.tipoObjeto.TryGetValue(cosa, out propiedad))
                && instancias.TryGetValue(cosa, out container))
        {
            if(requisito.Equals(propiedad) && instancias[cosa].Count != 0)
            { 
                foreach(string objeto in instancias[cosa])
                    if(objeto.Contains(cosa)) return condition;
                return !condition;
            } else if(instancias[cosa].Count == 0)  return !condition;
            if (instancias[requisito].Count != 0) return condition;

            return !condition;
        }
        
        return !condition;
    }

    protected float GetOntologyElement(string meta, bool isFinal = false)
    {
        float result = 0f; string strElement = "";

        foreach(Tuple<string, float> element in DataMeta.dicGoalOntology[meta])
        {
            if(element.Item2 > result && instancias[element.Item1].Count != 0){
                strElement = element.Item1;
                result = element.Item2;
            }
        }
        if(isFinal) elemento = strElement;
        
        return result;
    }

    protected virtual float ElementoDistancia(string meta, bool isFinal = false)
    {
        if(DataMeta.dicGoals[meta].objetivo.Equals(Util.StrEnum(Objetivo.Instantaneo))) 
            return 1;
        else if(DataMeta.dicGoals[meta].objetivo.Equals(Util.StrEnum(Objetivo.Dinamico))) 
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

        return 0.5f;
    }

    protected virtual void Ejecutar()
    {
        //ejecutandoMeta = true;

        switch (Meta_)
        {
            default:
                Util.Print(Meta_, isDebug);
                break;
        }
        //ejecutandoMeta = false;
        IniciarDeliberacion();
        if(metaSelected.Equals("")) IniciarDeliberacion();
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
        string strValues, distribution = "";
        float values, subValues = 0, first = 0, second = 0;

        for (int x = 0; x < persValues.Length; x++)
        {
            strValues = UnityEngine.Random.Range(0f, 1f).ToString("0.00");
            values = float.Parse(strValues);
            if (values < 0.25) values = 0.25f;//min ParValues = 0.5

            if (x % 2 == 0)
            {
                subValues = values * 2;
                distribution = UnityEngine.Random.Range(0f, 1f).ToString("0.00");

                first = subValues * float.Parse(distribution);

                if (first > 1) first = 1;

                persValues[x] = double.Parse(first.ToString("0.00"));
            }
            else
            {
                second = subValues - (subValues * float.Parse(distribution));

                if (second > 1) second = 1;

                persValues[x] = float.Parse(second.ToString("0.00"));
            }
            //Console.WriteLine(persValues[x]);
        }
        ParsePersonality();//Debug.Log(myPersAttributes);
    }

    /*public void CreatePersonality()
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
        //Debug.Log(myPersAttributes);
    }*/

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

    public double Puntua(string persAtributo)
    {
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
