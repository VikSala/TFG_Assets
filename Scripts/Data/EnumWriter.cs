using System;
using Mono.Data.Sqlite;
using Arcspark.DataToolkit;
using System.IO;
using System.Collections.Generic;

public class EnumWriter
{
    public void DatosEnumToFile(SQLiteConnection sqliteDB, string table, string outputPath)
    {
        List<string> listEnum = new List<string>{};
        List<string> listMeta = new List<string>{};
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

                            listEnum.Add(nombre);

                            // Escribir el nombre del enum
                            writer.WriteLine($"public enum {nombre}");
                            writer.WriteLine("{");

                            // Dividir los elementos y escribirlos como valores del enum
                            string[] elementosSeparados = elementos.Split('_');

                            for (int i = 0; i < elementosSeparados.Length; i++)
                            {
                                if (i < elementosSeparados.Length - 1)
                                {
                                    if(nombre.Equals("Meta")) listMeta.Add(elementosSeparados[i+1]);
                                    writer.WriteLine($"    {elementosSeparados[i]},");
                                }
                                else
                                {
                                    writer.WriteLine($"    {elementosSeparados[i]}");
                                }
                            }

                            writer.WriteLine("}");
                            writer.WriteLine();
                        }
                        catch (Exception){}
                    }
                }
                while (reader.NextResult());
            }
            UpdateRegionContent(sqliteDB, "Assets/TFG_Assets/Scripts/Data/DataUtil.cs", listEnum.ToArray(), listMeta.ToArray());
            UpdateRegionContent(sqliteDB, "Assets/TFG_Assets/Scripts/Data/DataMeta.cs", listMeta.ToArray());
        }
        catch (Exception) { }
    }

    string GetObjetos(SQLiteConnection sqliteDB)
    {
        string data = 
        "    static Dictionary<string, string> GetTipoObjeto()\n"+
        "    { return\n"+
        "        new Dictionary<string, string>(){ \n";

        SqliteDataReader reader = sqliteDB.SelectTable("Objeto");

        do
        {
            while (reader.Read())
            {
                try
                {
                    data += "            { StrEnum(Objeto."+reader.GetString("Nombre")+"), StrEnum(Tipo."+reader.GetString("Tipo")+") },\n";
                }
                catch (Exception){}
            }
        }
        while (reader.NextResult());

        data = data.Remove(data.Length - 2);//eliminamos la ultima coma
        data += "\n        };\n    }";
        return data;
    }

    public void DatosMetaToFile(SQLiteConnection sqliteDB, string table, string outputPath)
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
                            string etiquetas = reader.GetString("Etiquetas");
                            string prerrequisitos = reader.GetString("Prerrequisitos");
                            string objetivo = reader.GetString("Objetivo");
                            string rasgo = reader.GetString("Rasgo");

                            // Escribir el nombre del enum
                            writer.WriteLine($"public enum {nombre}");
                            writer.WriteLine("{");

                            writer.WriteLine($"    {etiquetas},");
                            writer.WriteLine($"    {prerrequisitos},");
                            writer.WriteLine($"    {objetivo},");
                            writer.WriteLine($"    {rasgo}");

                            writer.WriteLine("}");
                            writer.WriteLine();
                        }
                        catch (Exception){}
                    }
                }
                while (reader.NextResult());
            }
        }
        catch (Exception) { }
    }

    public void UpdateRegionContent(SQLiteConnection sqliteDB, string scriptPath, string[] enums, string[] metas)
    {
        bool actualizar = false;
        string[] lines = File.ReadAllLines(scriptPath);
        int enumCount = 0, metaCount = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("#region Update")) 
            {actualizar = true; lines[i] += "\n"+ GetObjetos(sqliteDB);}
            else if (actualizar)
            {
                if(enumCount < enums.Length)
                {
                    lines[i] = "    public static string[] strEnum"+enums[enumCount]+" = Enum.GetNames(typeof("+enums[enumCount]+"));";
                    i++;
                    lines[i] = "    public static string StrEnum("+enums[enumCount]+" p1) { return strEnum"+enums[enumCount]+"[(int)p1]; }\n";
                    enumCount++;
                }else
                {
                    if(metaCount < metas.Length){
                        lines[i] = "    public static string[] strEnum"+metas[metaCount]+" = Enum.GetNames(typeof("+metas[metaCount]+"));";
                    }
                    else {
                        lines[i] = "\n    #endregion Update\n}";
                        Array.Resize(ref lines, i + 1);
                        actualizar = false;
                    }
                    metaCount++;
                }
            }
        }

        // Escribe las líneas actualizadas de nuevo en el archivo
        File.WriteAllLines(scriptPath, lines);
    }

    string GetPesos(SQLiteConnection sqliteDB)
    {
        string data =
            "    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()\n" +
            "        { return\n" +
            "            new Dictionary<string, HashSet<Tuple<string, float>>>{\n";

        SqliteDataReader reader = sqliteDB.SelectTable("Peso", new string[] { "Meta", "Elemento", "Valor" }, "Meta IS NOT NULL ORDER BY Meta ASC");

        string currentMeta = "";
        List<string> elementosYPesos = new List<string>();

        while (reader.Read())
        {
            string meta = reader.GetString("Meta");
            string elemento = reader.GetString("Elemento");
            string peso = reader.GetString("Valor");

            if (currentMeta != meta)
            {
                if (currentMeta != "")
                {
                    data += "            { \"" + currentMeta + "\", new HashSet<Tuple<string, float>>(){\n";
                    foreach (string ep in elementosYPesos)
                    {
                        string[] parts = ep.Split(',');
                        data += "                Tuple.Create(\"" + parts[0] + "\", " + parts[1] + "f),\n";
                    }
                    data = data.Remove(data.Length - 2);
                    data += "\n            }},\n";
                }
                currentMeta = meta;
                elementosYPesos.Clear();
            }
            elementosYPesos.Add(elemento + "," + peso);
        }

        if (currentMeta != "")
        {
            data += "            { \"" + currentMeta + "\", new HashSet<Tuple<string, float>>(){\n";
            foreach (string ep in elementosYPesos)
            {
                string[] parts = ep.Split(',');
                data += "                Tuple.Create(\"" + parts[0] + "\", " + parts[1] + "f),\n";
            }
            data = data.Remove(data.Length - 2);
            data += "\n             }}\n";
        }

        data += "        };\n    }";

        return data;
    }

    public void UpdateRegionContent(SQLiteConnection sqliteDB, string scriptPath, string[] metas)
    {
        bool actualizar = false;
        string[] lines = File.ReadAllLines(scriptPath);
        int metaCount = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("#region Update"))
            {
                actualizar = true;
                lines[i] += "\n"+ GetPesos(sqliteDB);
                lines[i] += "\n    public static Dictionary<string, Data> dicGoals = new Dictionary<string, Data>(){";
            }
            else if (actualizar)
            {
                if(metaCount < metas.Length){
                    lines[i] = 
                    "        { Util.StrEnum(Meta."+metas[metaCount]+"),\n"+
                    "        new Data {  etiquetas = Util.strEnum"+metas[metaCount]+"[0].Split('_'),\n"+
                    "                    prerequisitos = Util.strEnum"+metas[metaCount]+"[1].Split('_'),\n"+
                    "                    objetivo = Tuple.Create(Util.strEnum"+metas[metaCount]+"[2].Split('_')[0], Util.strEnum"+metas[metaCount]+"[2].Split('_')[1]),\n"+
                    "                    rasgo = Util.strEnum"+metas[metaCount]+"[3]} }";
                    if(metaCount < metas.Length-1) lines[i] += ",";
                    else lines[i] += "\n};";
                }
                else {
                    lines[i] = "\n    #endregion Update\n}";
                    Array.Resize(ref lines, i + 1);
                    actualizar = false;
                }
                metaCount++;
            }
        }

        // Escribe las líneas actualizadas de nuevo en el archivo
        File.WriteAllLines(scriptPath, lines);
    }
}

public class EnumExporter
{
    private SQLiteConnection sqliteDB;

    public EnumExporter(SQLiteConnection dbConnection)
    {
        sqliteDB = dbConnection;
    }

    public void ExportEnums(string tabla)
    {
        EnumWriter enumWriter = new EnumWriter();
        string outputPath = "Assets/TFG_Assets/Scripts/Data/Datos" + tabla + ".cs";

        switch(tabla)
        {
            case "Enum":
                enumWriter.DatosEnumToFile(sqliteDB, tabla, outputPath);
            break;
            case "Meta":
                enumWriter.DatosMetaToFile(sqliteDB, tabla, outputPath);
            break;
        }
    }
}
