using System;
using System.Collections.Generic;

public class Util
{
    static string[] strEnumExistencia = Enum.GetNames(typeof(EstadoAgenteExistencia));
    static string[] strEnumRealidad = Enum.GetNames(typeof(EstadoAgenteRealidad));
    static string[] strEnumMetas = Enum.GetNames(typeof(MetasAgente));
    static string[] strEnumObjetos = Enum.GetNames(typeof(Objeto));
    static string[] strEnumLugares = Enum.GetNames(typeof(Lugar));
    static string[] strEnumRasgos = Enum.GetNames(typeof(Rasgo));
    static string[] strEnumPropiedades = Enum.GetNames(typeof(Propiedad));
    public static Dictionary<string, string> objetoPropiedad = GetObjetoPropiedad();
    
    public static string StrEnum(MetasAgente p1 = MetasAgente.SinValor)
    { return strEnumMetas[(int)p1]; }
    public static string StrEnum(EstadoAgenteExistencia p1 = EstadoAgenteExistencia.SinValor)
    { return strEnumExistencia[(int)p1]; }
    public static string StrEnum(EstadoAgenteRealidad p1 = EstadoAgenteRealidad.SinValor)
    { return strEnumRealidad[(int)p1]; }
    public static string StrEnum(Objeto p1 = Objeto.SinValor)
    { return strEnumObjetos[(int)p1]; }
    public static string StrEnum(Lugar p1 = Lugar.SinValor)
    { return strEnumLugares[(int)p1]; }
    public static string StrEnum(Rasgo p1)
    { return strEnumRasgos[(int)p1]; }
    public static string StrEnum(Propiedad p1)
    { return strEnumPropiedades[(int)p1]; }

    static Dictionary<string, string> GetObjetoPropiedad()
    { return
        new Dictionary<string, string>(){ 
            { StrEnum(Objeto.Manos), StrEnum(Propiedad.Herramienta) },
            { StrEnum(Objeto.Azada), StrEnum(Propiedad.Herramienta) },
            { StrEnum(Objeto.Lanza), StrEnum(Propiedad.Herramienta) },
            { StrEnum(Objeto.Carne), StrEnum(Propiedad.Comida) },
            { StrEnum(Objeto.Baya), StrEnum(Propiedad.Comida) },
            { StrEnum(Objeto.Agua), StrEnum(Propiedad.Bebida) },
            { StrEnum(Lugar.Gremio), StrEnum(Lugar.Gremio) },
            { StrEnum(EstadoAgenteRealidad.Recurso), StrEnum(EstadoAgenteRealidad.Recurso) }
        }; 
    }

    public static void Print(string msg, bool debugMsg = true){ if(debugMsg) UnityEngine.Debug.Log(msg); }
}

// Enumeración de las metas posibles del agente
public enum MetasAgente
{
    SinValor, 
    Comer,//Comida: Recurso
    Beber,//Bebida: Recurso
    Dormir,//Cama_Gremio: Lugar
    Atacar,//Herramienta
    Huir,//Peligro
    Recolectar,//Herramienta, Lugar: Vector3
    Cocinar,//Recurso, Lugar: Vector3
    Comerciar//Recurso, Lugar
}

// Enumeración de los estados posibles del agente
public enum EstadoAgenteExistencia
{
    SinValor, 
    Hambre,     //El agente tiene hambre 
    Sed,        //El agente tiene sed 
    Somnolencia,    //El agente tiene sueño 
    Amenaza,    //El agente detecta amenaza 
    Peligro     //El agente detecta peligro
}

public enum EstadoAgenteRealidad
{
    SinValor, 
    Recurso,    //El agente detecta un recurso
    Agente      //El agente detecta un agente
}

public enum EstadoAgenteBiologico
{
    SinValor, 
    ConSed,
    Sediento,
    ConHambre,
    Hambriento,
    SinHambre,
    Alimentado,
    SinSed,
    Hidratado,
    Cansado,
    Descansado
}

public enum Rasgo
{
    Explorador, Conservador,
    Extrovertido, Introvertido,
    Escrupuloso, Despreocupado,
    Altruista, Egocentrico,
}

public enum Objeto
{
    SinValor, 
    Manos,
    Azada,
    Lanza,
    Carne,
    Baya,
    Agua
}

public enum Propiedad
{
    SinValor, 
    Herramienta,
    Comida,
    Bebida
}

public enum Lugar
{
    SinValor, 
    Gremio,
    Cocina,
    Bosque,
    Cama,
    Lago
}