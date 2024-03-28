using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Util : MonoBehaviour
{
    #region Update
    static Dictionary<string, string> GetTipoObjeto()
    { return
        new Dictionary<string, string>(){ 
            { StrEnum(Objeto.Manos), StrEnum(Tipo.Herramienta) },
            { StrEnum(Objeto.Hoz), StrEnum(Tipo.Herramienta) },
            { StrEnum(Objeto.Espada), StrEnum(Tipo.Herramienta) },
            { StrEnum(Objeto.Carne), StrEnum(Tipo.Comida) },
            { StrEnum(Objeto.Baya), StrEnum(Tipo.Comida) },
            { StrEnum(Objeto.Agua), StrEnum(Tipo.Bebida) }
        };
    }
    public static string[] strEnumPercepcion = Enum.GetNames(typeof(Percepcion));
    public static string StrEnum(Percepcion p1) { return strEnumPercepcion[(int)p1]; }

    public static string[] strEnumEstado = Enum.GetNames(typeof(Estado));
    public static string StrEnum(Estado p1) { return strEnumEstado[(int)p1]; }

    public static string[] strEnumRasgo = Enum.GetNames(typeof(Rasgo));
    public static string StrEnum(Rasgo p1) { return strEnumRasgo[(int)p1]; }

    public static string[] strEnumObjeto = Enum.GetNames(typeof(Objeto));
    public static string StrEnum(Objeto p1) { return strEnumObjeto[(int)p1]; }

    public static string[] strEnumObjetivo = Enum.GetNames(typeof(Objetivo));
    public static string StrEnum(Objetivo p1) { return strEnumObjetivo[(int)p1]; }

    public static string[] strEnumTipo = Enum.GetNames(typeof(Tipo));
    public static string StrEnum(Tipo p1) { return strEnumTipo[(int)p1]; }

    public static string[] strEnumLugar = Enum.GetNames(typeof(Lugar));
    public static string StrEnum(Lugar p1) { return strEnumLugar[(int)p1]; }

    public static string[] strEnumEntidad = Enum.GetNames(typeof(Entidad));
    public static string StrEnum(Entidad p1) { return strEnumEntidad[(int)p1]; }

    public static string[] strEnumMeta = Enum.GetNames(typeof(Meta));
    public static string StrEnum(Meta p1) { return strEnumMeta[(int)p1]; }

    public static string[] strEnumComer = Enum.GetNames(typeof(Comer));
    public static string[] strEnumBeber = Enum.GetNames(typeof(Beber));
    public static string[] strEnumDormir = Enum.GetNames(typeof(Dormir));
    public static string[] strEnumAtacar = Enum.GetNames(typeof(Atacar));
    public static string[] strEnumHuir = Enum.GetNames(typeof(Huir));
    public static string[] strEnumRecolectar = Enum.GetNames(typeof(Recolectar));
    public static string[] strEnumCocinar = Enum.GetNames(typeof(Cocinar));
    public static string[] strEnumComerciar = Enum.GetNames(typeof(Comerciar));
    public static string[] strEnumIrLago = Enum.GetNames(typeof(IrLago));

    #endregion Update
}
