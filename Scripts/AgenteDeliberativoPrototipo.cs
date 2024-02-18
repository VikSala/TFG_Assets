using System;
using UnityEngine;
using System.Collections.Generic;

public partial class AgenteDeliberativoPrototipo : MonoBehaviour
{
    HashSet<string> listDeseos = new HashSet<string>{};
    //string metaActual = Util.StrEnum(MetasAgente.SinValor);//Util.StrEnum(MetasAgente.Comerciar);
    public bool compile = false; bool isBreak = false, interrumpir = false;

    [NonSerialized]
    public GameObject ObjetivoTemporal, ObjetivoTemporalFinal;
    Personalidad yo;    string elemento = ""; Vector3 vectorObjetivo;// = Vector3.zero; //NodoMeta nodoMeta; 
    //EstadoAgenteBiologico estadoAgente = EstadoAgenteBiologico.SinValor;
    public LugarManager lugarManager;
    string nuevaNecesidad = "", metaSelected = "";
    [NonSerialized]
    public string necesidadActual = "";
    bool mensajeRecibido = false; 
    [NonSerialized]
    public bool ejecutandoMeta = false;

    //Conocimiento universal
    //Dictionary<string, DataGoals.Data> dicGoals = DataGoals.dicGoals;
    //Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = DataGoals.dicGoalOntology;

    //Memoria
//la estructura memoria unifica los conceptos mem. largo plazo y corto plazo:
//- corto plazo: actualizacion de estados biologicos
//- largo plazo: el resto de conceptos son read_only
//Instancias tambien lo hace lp:lugares y cp:objetos
    public HashSet<string> memoria = new HashSet<string>
        {Util.StrEnum(Objeto.Manos), Util.StrEnum(Objeto.Azada), Util.StrEnum(Objeto.Agua), Util.StrEnum(Objeto.Lanza),
         Util.StrEnum(Objeto.Carne), Util.StrEnum(Lugar.Gremio), Util.StrEnum(EstadoAgenteRealidad.Recurso), 
         Util.StrEnum(Objeto.Baya), Util.StrEnum(EstadoAgenteBiologico.SinHambre), Util.StrEnum(EstadoAgenteBiologico.SinSed),
         Util.StrEnum(EstadoAgenteBiologico.Descansado), Util.StrEnum(EstadoAgenteExistencia.Amenaza)};//Alimentado, Hidratado, Descansado

    public Dictionary<string, HashSet<string>> instancias = new Dictionary<string, HashSet<string>>(){
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


    /*              Deliberacion
	Util:
    - Seed
	*/

    void Awake()
    {
        yo = new Personalidad();
        InvokeRepeating("IniciarDeliberacion", 0f, Util.frecuencia);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            finalizar = true;
            ObjetivoRandom = Vector3.zero;
            Ejecutar();
        }
    }

    void OnValidate()
    {
        if(compile){
            string laMeta = Util.StrEnum(MetasAgente.Atacar);
            print("Meta " + laMeta + " tiene viabilidad: " + MetaViable(laMeta));
            compile = false;
        }
    }

    void IniciarDeliberacion()
    {
        metaSelected = "";
        double result = 0; double finalResult = 0;

        if(mensajeRecibido || !ejecutandoMeta || interrumpir){
            foreach(var kv in DataGoals.dicGoals)
            {
                foreach(string etiqueta in kv.Value.etiquetas)
                {
                    if(isBreak) {isBreak = false; break;};if(interrumpir) {interrumpir = false;};
                    if(nuevaNecesidad.Equals(etiqueta) || necesidadActual.Equals(etiqueta) || listDeseos.Count == 0)
                        if(MetaViable(kv.Key)){
                            //print("Meta viable: "+ kv.Key);//
                            if(!Meta_.Equals(kv.Key)) result += yo.Puntua(kv.Value.rasgo, memoria);
                            else  result += yo.Puntua(kv.Value.rasgo, memoria)/2;
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

    public void IniciarMeta(string meta)
    {
        foreach(string etiqueta in DataGoals.dicGoals[metaSelected].etiquetas) 
            if(listDeseos.Contains(etiqueta))
            {
                listDeseos.Remove(etiqueta);
                break;
            }

        Meta_ = meta;
        Objeto_ = elemento;
        Objetivo_ = vectorObjetivo;

        Ejecutar();
        /*nodoMeta = new NodoMeta
        {
            Meta_ = meta,
            Objeto_ = elemento,
            Objetivo = vectorObjetivo
        };
        ejecutandoMeta = true;
        nodoMeta.Ejecutar();*/
    }

    public void newMessage(string msg)
    {
        //print("Mensaje: " + msg + " Enviado");
        if(!listDeseos.Contains(msg)){
            listDeseos.Add(msg);
            necesidadActual = nuevaNecesidad;
            nuevaNecesidad = msg;
            mensajeRecibido = true;
            print("Mensaje: " + msg + " Recibido");
        }
        mensajeRecibido = false;
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

    bool MetaViable(string meta)
    {
        bool result = false; bool and = false;

        foreach(string requisito in DataGoals.dicGoals[meta].prerequisitos)
        {
            foreach(string cosa in memoria)
            {
                if(CheckCondition(requisito, cosa)) {result = true; break;}//result = true;
                else if(and && !requisito.Equals(Util.AND)) return false;
                if(requisito.Equals(Util.AND))
                    if(!result) return false;
                    else {and = true; break;}//and = true;
            }
        }

        return result;
    }

    bool CheckCondition(string requisito, string cosa)
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
            else if(Util.objetoPropiedad.TryGetValue(cosa, out propiedad) && !requisito.Equals(propiedad)) return !condition;
        
        if ((instancias.TryGetValue(requisito, out container) || Util.objetoPropiedad.TryGetValue(cosa, out propiedad))
                && instancias.TryGetValue(cosa, out container))
        {
            //if (Util.objetoPropiedad.TryGetValue(cosa, out propiedad))
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

    float GetOntologyElement(string meta, bool isFinal = false)
    {
        float result = -1f; string strElement = "";

        foreach(Tuple<string, float> element in DataGoals.dicGoalOntology[meta])
        {
            if(element.Item2 > result && instancias[element.Item1].Count != 0){
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
               
                //ObjetivoTemporal = null;
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
