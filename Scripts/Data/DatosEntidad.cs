using System.Collections.Generic;
using UnityEngine;

public class DatosEntidad : MonoBehaviour{              //SQLITE: Agente
    [SerializeField]
    private string nombre;
    
    [SerializeField]
    private float frecuencia;

    [SerializeField]
    private int resets;
    
    public List<string> rasgosPersonalidad = new List<string>();

    public string Nombre {
        get { return nombre; }
        set { nombre = value; }
    }

    public float Frecuencia {
        get { return frecuencia; }
        set { frecuencia = value; }
    }

    public int Resets {
        get { return resets; }
        set { resets = value; }
    }

    public List<string> RasgosPersonalidad {
        get { return rasgosPersonalidad; }
        set { rasgosPersonalidad = value; }
    }
}

/*public class ContenedorDatos : MonoBehaviour {
    [SerializeField]
    private DatosEntidad datos;

    // Método para acceder a los datos desde otros scripts si es necesario
    public DatosEntidad ObtenerDatos() {
        return datos;
    }

    // Método para establecer los datos desde otros scripts si es necesario
    public void EstablecerDatos(DatosEntidad nuevosDatos) {
        datos = nuevosDatos;
    }
}*/
