using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Util : MonoBehaviour
{
    public static int seed; public int semilla; public bool useSeed = true; 
    public static float frecuencia = 0.25f;
    public static string AND = "1"; public static char NOT = '0';
    public static Dictionary<string, string> tipoObjeto = GetTipoObjeto();

    void Awake()
    {
        if(!useSeed) semilla = Environment.TickCount;
        seed = semilla;//1886060671;//Environment.TickCount;
        UnityEngine.Random.InitState(seed);//print("Seed: " + seed);
        //frecuencia = UnityEngine.Random.Range(0f, 0.25f);//print("frecuencia: " + frecuencia);
    }

    public static void Print(string msg, bool debugMsg = true){ if(debugMsg) Debug.Log(msg); }
}

public enum Tiempo
{
    Corto = 1,
    Medio = 3,
    Largo = 6
}