using System;
using System.Collections.Generic;

public class DataMeta              //SQLITE: Metas
{
    public static Dictionary<string, HashSet<Tuple<string, float>>> dicGoalOntology = GetGoalElementsOntology();
    public struct Data
    {
        public string rasgo;
        public string[] etiquetas;
        public string[] prerrequisitos;
        public Tuple<string, string> objetivo;
    }

    #region Update
    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()
        { return
            new Dictionary<string, HashSet<Tuple<string, float>>>{
            { "Atacar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Manos", 0.5f),
                Tuple.Create("Hoz", 0.75f),
                Tuple.Create("Espada", 1.0f),
                Tuple.Create("Amenaza", 1.0f)
            }},
            { "Beber", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Agua", 1.0f)
            }},
            { "Cocinar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Baya", 0.5f),
                Tuple.Create("Carne", 1.0f)
            }},
            { "Comer", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Baya", 0.5f),
                Tuple.Create("Carne", 1.0f)
            }},
            { "Comerciar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Baya", 1.0f),
                Tuple.Create("Carne", 0.5f)
            }},
            { "Dormir", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Gremio", 1.0f)
            }},
            { "Huir", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Gremio", 1.0f)
            }},
            { "IrLago", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Lago", 1.0f)
            }},
            { "Recolectar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Manos", 0.5f),
                Tuple.Create("Hoz", 1.0f),
                Tuple.Create("Recurso", 1.0f)
             }}
        };
    }
    public static Dictionary<string, Data> dicGoals = new Dictionary<string, Data>(){
        { Util.StrEnum(Meta.Comer),
        new Data {  etiquetas = Util.strEnumComer[0].Split('_'),
                    prerrequisitos = Util.strEnumComer[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumComer[2].Split('_')[0], Util.strEnumComer[2].Split('_')[1]),
                    rasgo = Util.strEnumComer[3]} },
        { Util.StrEnum(Meta.Beber),
        new Data {  etiquetas = Util.strEnumBeber[0].Split('_'),
                    prerrequisitos = Util.strEnumBeber[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumBeber[2].Split('_')[0], Util.strEnumBeber[2].Split('_')[1]),
                    rasgo = Util.strEnumBeber[3]} },
        { Util.StrEnum(Meta.Dormir),
        new Data {  etiquetas = Util.strEnumDormir[0].Split('_'),
                    prerrequisitos = Util.strEnumDormir[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumDormir[2].Split('_')[0], Util.strEnumDormir[2].Split('_')[1]),
                    rasgo = Util.strEnumDormir[3]} },
        { Util.StrEnum(Meta.Atacar),
        new Data {  etiquetas = Util.strEnumAtacar[0].Split('_'),
                    prerrequisitos = Util.strEnumAtacar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumAtacar[2].Split('_')[0], Util.strEnumAtacar[2].Split('_')[1]),
                    rasgo = Util.strEnumAtacar[3]} },
        { Util.StrEnum(Meta.Huir),
        new Data {  etiquetas = Util.strEnumHuir[0].Split('_'),
                    prerrequisitos = Util.strEnumHuir[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumHuir[2].Split('_')[0], Util.strEnumHuir[2].Split('_')[1]),
                    rasgo = Util.strEnumHuir[3]} },
        { Util.StrEnum(Meta.Recolectar),
        new Data {  etiquetas = Util.strEnumRecolectar[0].Split('_'),
                    prerrequisitos = Util.strEnumRecolectar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumRecolectar[2].Split('_')[0], Util.strEnumRecolectar[2].Split('_')[1]),
                    rasgo = Util.strEnumRecolectar[3]} },
        { Util.StrEnum(Meta.Cocinar),
        new Data {  etiquetas = Util.strEnumCocinar[0].Split('_'),
                    prerrequisitos = Util.strEnumCocinar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumCocinar[2].Split('_')[0], Util.strEnumCocinar[2].Split('_')[1]),
                    rasgo = Util.strEnumCocinar[3]} },
        { Util.StrEnum(Meta.Comerciar),
        new Data {  etiquetas = Util.strEnumComerciar[0].Split('_'),
                    prerrequisitos = Util.strEnumComerciar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumComerciar[2].Split('_')[0], Util.strEnumComerciar[2].Split('_')[1]),
                    rasgo = Util.strEnumComerciar[3]} },
        { Util.StrEnum(Meta.IrLago),
        new Data {  etiquetas = Util.strEnumIrLago[0].Split('_'),
                    prerrequisitos = Util.strEnumIrLago[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumIrLago[2].Split('_')[0], Util.strEnumIrLago[2].Split('_')[1]),
                    rasgo = Util.strEnumIrLago[3]} }
};

    #endregion Update
}
