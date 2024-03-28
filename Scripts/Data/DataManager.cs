using UnityEngine;
using Arcspark.DataToolkit;

public class DataManager : MonoBehaviour 
{
    public bool actualizar = false;

    void OnValidate()
    {
        if(actualizar){
            ActualizarDatos();
            actualizar = false;
        }
    }

    private void ActualizarDatos()
    {
        SQLiteConnection sqliteDB = new SQLiteConnection(Application.streamingAssetsPath+ "/Samples/Data Toolkit/test.db");

        DataExporter dataExporter = new DataExporter(sqliteDB);
        dataExporter.ExportData("Enum"); 
        dataExporter.ExportData("Meta");
        print("Datos actualizados.");

        sqliteDB.Close();
    }
}
