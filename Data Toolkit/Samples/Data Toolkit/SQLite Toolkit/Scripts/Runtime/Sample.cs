using Mono.Data.Sqlite;
using UnityEngine;
using Arcspark.DataToolkit;
using System.IO;

namespace Arcspark.Sample.SQLiteToolkitSample
{
    public class Sample : MonoBehaviour
    {
        string tabla = "Metas";
        public bool compile = false;

        private void Awake()
        {
            Init();
        }

        void OnValidate()
        {
            if(compile){
                Test();
                compile = false;
            }
        }

        private void Test()
        {
            SQLiteConnection sqliteDB = new SQLiteConnection(Application.streamingAssetsPath+ "/Samples/Data Toolkit/test.db");

            DataExporter dataExporter = new DataExporter(sqliteDB);
            dataExporter.ExportData("Enum"); 
            dataExporter.ExportData("Meta");
            print("Updated enums.");

            sqliteDB.Close();
        }

        // Sample Use SQLite Helper
        private void Init()
        {
            SQLiteConnection sqliteDB = new SQLiteConnection(DBConnectString);

            //PrintTableUnit(sqliteDB);
            AddNewTable(sqliteDB);
            //InsertSomeAccount(sqliteDB);
            //DeleteSomeAccount0(sqliteDB);
            //DeleteSomeAccount1(sqliteDB);
            //UpdateSomeAccount(sqliteDB);
            //DeleteTable(sqliteDB);

            sqliteDB.Close();
        }

        private void AddNewTable(SQLiteConnection sqliteDB)
        {
            sqliteDB.CreateTable("Agente",
                new string[] { "text NOT NULL", "real", "integer", "text" },
                new string[] { "Nombre", "Frecuencia", "Resets", "Personalidad" }
            );

            Debug.Log("== Add New Table Finished ==");
        }

        private void InsertSomeAccount(SQLiteConnection sqliteDB, string table)
        {
            sqliteDB.InsertValues(table, new string[] { "'"+"Agente_1_seed"+"'", "frecuencia", "resets", "'"+"myPersAttributes"+"'" });

            Debug.Log("== Insert New Values Finished ==");

            //PrintTableUnit1(sqliteDB);
        }

        void InsertarAgente(string table)
        {
            SQLiteConnection sqliteDB = new SQLiteConnection(DBConnectString);
            sqliteDB.InsertValues(table, new string[] { "'"+"Agente_1_seed"+"'", "frecuencia", "resets", "'"+"myPersAttributes"+"'" });

            Debug.Log("== Insert New Values Finished ==");

            sqliteDB.Close();
        }

        private void DeleteSomeAccount0(SQLiteConnection sqliteDB)
        {
            sqliteDB.DeleteValuesOR("Unit1",
                new string[] { "Balance", "Card_Name" },
                new string[] { "<", "=" },
                new string[] { "0", "'AmericanExpress'" }
            );

            Debug.Log("== Delete Values Finished: Part 0==");

            PrintTableUnit1(sqliteDB);
        }

        private void DeleteSomeAccount1(SQLiteConnection sqliteDB)
        {
            sqliteDB.DeleteValuesAND("Unit1",
                new string[] { "Balance", "Certificate_Timestamp" },
                new string[] { ">=", "<>" },
                new string[] { "2000", "'2004-8-22'" }
            );

            Debug.Log("== Delete Values Finished: Part 1 ==");

            PrintTableUnit1(sqliteDB);
        }

        private void UpdateSomeAccount(SQLiteConnection sqliteDB)
        {
            sqliteDB.UpdateValuesAND("Unit1",
                new string[] {"Card_Number"},
                new string[] {"="},
                new string[] {"20000"},
                new string[] { "Card_Number", "Card_Name", "Balance" },
                new string[] { "0", "N/A", "0" }
            );

            Debug.Log("== Update Values Finished ==");

            PrintTableUnit1(sqliteDB);
        }

        private void DeleteTable(SQLiteConnection sqliteDB)
        {
            sqliteDB.DropTable("Unit1");

            Debug.Log("== Delete Table Finished ==");
        }

        private void PrintTableUnit(SQLiteConnection sqliteDB)
        {
            Debug.Log("== Print DB Unit ==");

            SqliteDataReader reader = sqliteDB.SelectTable("Unit");                                                                                                                                                                                                                                   
            do
            {
                while (reader.Read())
                {
                    try
                    {
                        int? id = reader.GetInt32("ID");
                        string name = reader.GetString("Name");
                        double? attackDamage = reader.GetDouble("Attack_Damage");

                        Debug.Log(string.Format("ID: {0}, Name: {1}, AttackDamage: {2}", id, name, attackDamage));
                    }
                    catch (SqliteException e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            while (reader.NextResult());
        }

        private void PrintTableEnum(SQLiteConnection sqliteDB, string table)
        {
            Debug.Log("== Print DB Unit ==");

            SqliteDataReader reader = sqliteDB.SelectTable(table);                                                                                                                                                                                                                                   
            do
            {
                while (reader.Read())
                {
                    try
                    {
                        string nombre = reader.GetString("Nombre");
                        string elementos = reader.GetString("Elementos");

                        Debug.Log(string.Format("Nombre: {1}, Elementos: {2}", nombre, elementos));
                    }
                    catch (SqliteException e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            while (reader.NextResult());
        }

        private void WriteEnumsToFile(SQLiteConnection sqliteDB, string table, string outputPath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    SqliteDataReader reader = sqliteDB.SelectTable(table);
                    
                    do
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                string nombre = reader.GetString("Nombre");
                                string elementos = reader.GetString("Elementos");

                                // Escribir el nombre del enum
                                writer.WriteLine($"public enum {nombre}");
                                writer.WriteLine("{");

                                // Dividir los elementos y escribirlos como valores del enum
                                string[] elementosSeparados = elementos.Split('_');
                                foreach (string elemento in elementosSeparados)
                                {
                                    writer.WriteLine($"    {elemento},");
                                }

                                writer.WriteLine("}");
                                writer.WriteLine();
                            }
                            catch (SqliteException e)
                            {
                                Debug.Log(e.Message);
                            }
                        }
                    }
                    while (reader.NextResult());
                }

                Debug.Log("Enums escritos en el archivo con éxito.");
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error al escribir en el archivo: {ex.Message}");
            }
        }

        private void PrintTableUnit1(SQLiteConnection sqliteDB)
        {
            Debug.Log("== Print DB Unit1 ==");

            SqliteDataReader reader = sqliteDB.SelectTable("Unit1");
            do
            {
                while (reader.Read())
                {
                    try
                    {
                        int? cardNumber = reader.GetInt32("Card_Number");
                        string cardName = reader.GetString("Card_Name");
                        double? balance = reader.GetDouble("Balance");
                        string certificateTimestamp = reader.GetString("Certificate_Timestamp");

                        Debug.Log(string.Format("Card_Number: {0}, Card_Name: {1}, Balance: {2}, Certificate_Timestamp: {3}", cardNumber, cardName, balance, certificateTimestamp));
                    }
                    catch (SqliteException e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }
            while (reader.NextResult());
        }

        // Get Database from StreamingAssets Folder
        private string DBConnectString
        {
            get => Application.streamingAssetsPath+ "/Samples/Data Toolkit/test.db";
        }
    }
}