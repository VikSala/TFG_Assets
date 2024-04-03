using System.Collections.Generic;
using UnityEngine;

public class LugarManager : MonoBehaviour
{
    // Estructura para almacenar el nombre y la posición del lugar
    public struct InfoLugar
    {
        public string nombre;
        public Vector3 posicion;

        public InfoLugar(string nombre, Vector3 posicion)
        {
            this.nombre = nombre;
            this.posicion = posicion;
        }
    }
    public List<GameObject> Lugares = new List<GameObject>();
    HashSet<InfoLugar> info = new();

    [System.NonSerialized]
    public float radioPlano;
    public RandomPlaneSpawner rps;

    void Awake(){
        ObtenerLugares();
        radioPlano = rps.GetDistanciaPlano()/2;
    }

    public void ObtenerLugares()
    {
        foreach (GameObject lugar in Lugares)
        {
            info.Add(new InfoLugar(lugar.name, lugar.transform.position));
        }
    }

    // Método para obtener la posición de un lugar por su nombre
    public Vector3 ObtenerPosicionLugar(string nombreLugar)
    {
        foreach (InfoLugar lugar in info)
        {
            if (lugar.nombre.Equals(nombreLugar))
            {
                return lugar.posicion;
            }
        }
        Debug.LogError("No se encontró ningún lugar con el nombre: " + nombreLugar);
        return Vector3.zero;
    }
}
