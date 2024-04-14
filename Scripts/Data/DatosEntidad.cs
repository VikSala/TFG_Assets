using System.Collections.Generic;
using Arcspark.DataToolkit;
using UnityEngine;

public class DatosEntidad : MonoBehaviour{
    [SerializeField] private string nombre;
    
    [SerializeField] private float frecuencia;

    [SerializeField] private int resets;

    string rasgos;    
    public List<string> rasgosPersonalidad = new List<string>();

    int carne, baya, agua, comer, beber, dormir, atacar, huir, recolectar, cocinar, comerciar, irLago;

    public string Nombre {
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

    public string Rasgos {
        set { rasgos = value; }
    }

    public int Carne {
        get { return carne; }
        set { carne = value; }
    }
    public int Baya {
        get { return baya; }
        set { baya = value; }
    }
    public int Agua {
        get { return agua; }
        set { agua = value; }
    }
    public int Comer {
        get { return comer; }
        set { comer = value; }
    }
    public int Beber {
        get { return beber; }
        set { beber = value; }
    }
    public int Dormir {
        get { return dormir; }
        set { dormir = value; }
    }
    public int Atacar {
        get { return atacar; }
        set { atacar = value; }
    }
    public int Huir {
        get { return huir; }
        set { huir = value; }
    }
    public int Recolectar {
        get { return recolectar; }
        set { recolectar = value; }
    }
    public int Cocinar {
        get { return cocinar; }
        set { cocinar = value; }
    }
    public int Comerciar {
        get { return comerciar; }
        set { comerciar = value; }
    }
    public int IrLago {
        get { return irLago; }
        set { irLago = value; }
    }

    public List<string> RasgosPersonalidad {
        get { return rasgosPersonalidad; }
        set { rasgosPersonalidad = value; }
    }

    public bool compile;
    void OnValidate()
    {
        if(compile){
            GuardarResultados();
            compile = false;
        }
    }

    public void GuardarResultados()
    {
        SQLiteConnection sqliteDB = new SQLiteConnection(Application.streamingAssetsPath+ "/Samples/Data Toolkit/test.db");
        sqliteDB.InsertValues("Simulacion", 
            new string[] {  "'"+nombre+"'",
                            "'"+"" + CalcularFitness()+"'", 
                            "'"+"" + frecuencia+"'",
                            "'"+"" + resets+"'", 
                            "'"+rasgos+"'",
                            "'"+"" + carne+"'", 
                            "'"+"" + baya+"'", 
                            "'"+"" + agua+"'", 
                            "'"+"" + comer+"'", 
                            "'"+"" + beber+"'", 
                            "'"+"" + dormir+"'", 
                            "'"+"" + atacar+"'", 
                            "'"+"" + huir+"'", 
                            "'"+"" + recolectar+"'",
                            "'"+"" + cocinar+"'", 
                            "'"+"" + comerciar+"'",
                            "'"+"" + irLago+"'"
                        });

        //Debug.Log("== Insert New Values Finished ==");

        sqliteDB.Close();
    }

    public int CalcularFitness()
    {
        int fitness = GetComponent<AgenteDeliberativoSim>().GetBioValor();
        fitness = fitness - resets + carne + baya + agua;
        return fitness; 
    }
}
