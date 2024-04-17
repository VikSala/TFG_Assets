using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Util : MonoBehaviour
{
    public static int seed; public int semilla; public bool useSeed = false;
    public static bool compartirSemilla = false, multiSimLista = false;
    public static char NOT = '0';
    public static string AND = "1", SpawnTag = "Respawn", TerrainTag = "Terrain";
    /*
    SpawnTag = Escenas Prototipo
    TerrainTag = Terrain, SpawnPlane, Navigation
    */
    public static Dictionary<string, string> tipoObjeto = GetTipoObjeto();

    void Awake()
    {
        if(!useSeed) semilla = Environment.TickCount;
        seed = semilla;
        UnityEngine.Random.InitState(seed);
    }

    public static void Print(string msg, bool debugMsg = true){ if(debugMsg) Debug.Log(msg); }
}

public enum Tiempo
{
    Corto = 1,
    Medio = 3,
    Largo = 6
}