using System;
using System.Collections.Generic;

public class DataMeta              //SQLITE: Metas
{
    public static Dictionary<string, HashSet<Tuple<string, float>>> dicGoalOntology = GetGoalElementsOntology();
    public struct Data
    {
        public string rasgo;
        public string[] etiquetas;
        public string[] prerequisitos;
        public Tuple<string, string> objetivo;
    }
    
    #region Metas
    public static Dictionary<string, Data> dicGoal = new Dictionary<string, Data>()
	{ { Util.StrEnum(Meta.Comer), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Tipo.Comida), Util.AND, Util.NOT+Util.StrEnum(Estado.Alimentado)},
                    objetivo = Tuple.Create(Util.StrEnum(Tipo.Comida), Util.StrEnum(Objetivo.Instantaneo)),
                    rasgo = Util.StrEnum(Rasgo.Despreocupado)} }, 
      { Util.StrEnum(Meta.Beber), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Sed)},
                    prerequisitos = new string[]{Util.StrEnum(Tipo.Bebida), Util.AND, Util.NOT+Util.StrEnum(Estado.Hidratado)},
                    objetivo = Tuple.Create(Util.StrEnum(Tipo.Bebida), Util.StrEnum(Objetivo.Instantaneo)),
                    rasgo = Util.StrEnum(Rasgo.Conservador)} },
      { Util.StrEnum(Meta.Dormir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Somnolencia)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.NOT+Util.StrEnum(Estado.Descansado)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Introvertido)} },
      { Util.StrEnum(Meta.Atacar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Amenaza)},
                    prerequisitos = new string[]{Util.StrEnum(Tipo.Herramienta), Util.AND, Util.StrEnum(Percepcion.Amenaza)},
                    objetivo = Tuple.Create(Util.StrEnum(Objetivo.SinValor), Util.StrEnum(Objetivo.Dinamico)),
                    rasgo = Util.StrEnum(Rasgo.Egocentrico)} },
      { Util.StrEnum(Meta.Huir), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Amenaza)},
                    prerequisitos = new string[]{Util.StrEnum(Percepcion.Amenaza)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Altruista)} }, 
      { Util.StrEnum(Meta.Recolectar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Tipo.Herramienta)},
                    objetivo = Tuple.Create(Util.StrEnum(Percepcion.Recurso), Util.StrEnum(Objetivo.Dinamico)),
                    rasgo = Util.StrEnum(Rasgo.Explorador)} },
      { Util.StrEnum(Meta.Cocinar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Hambre)},
                    prerequisitos = new string[]{Util.StrEnum(Tipo.Comida)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Cocina), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Escrupuloso)} },
      { Util.StrEnum(Meta.Comerciar), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Sed)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Gremio), Util.AND, Util.StrEnum(Percepcion.Recurso), Util.NOT+Util.StrEnum(Estado.Hidratado)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Gremio), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Extrovertido)}},
      { Util.StrEnum(Meta.IrLago), 
        new Data {  etiquetas = new string[]{Util.StrEnum(Percepcion.Sed)},
                    prerequisitos = new string[]{Util.StrEnum(Lugar.Lago), Util.AND, Util.NOT+Util.StrEnum(Estado.Hidratado)},
                    objetivo = Tuple.Create(Util.StrEnum(Lugar.Lago), Util.StrEnum(Objetivo.Estatico)),
                    rasgo = Util.StrEnum(Rasgo.Explorador)}}
    };
    #endregion Metas

    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntologys()
    {
        Dictionary<string, HashSet<Tuple<string, float>>> dicGoalElementsOntology = new();
        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Comer), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Baya), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Carne), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Beber), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Agua), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Dormir), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Lugar.Gremio), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Atacar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Manos), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Azada), 0.75f),
            Tuple.Create(Util.StrEnum(Objeto.Lanza), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Huir), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Lugar.Gremio), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Recolectar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Manos), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Azada), 1.0f),
            Tuple.Create(Util.StrEnum(Percepcion.Recurso), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Cocinar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Baya), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Carne), 1.0f)});

        dicGoalElementsOntology.Add(Util.StrEnum(Meta.Comerciar), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Objeto.Carne), 0.5f),
            Tuple.Create(Util.StrEnum(Objeto.Baya), 1.0f)});
        
        dicGoalElementsOntology.Add(Util.StrEnum(Meta.IrLago), new HashSet<Tuple<string, float>>(){
            Tuple.Create(Util.StrEnum(Lugar.Lago), 1.0f)});
        
        return dicGoalElementsOntology;
    }

    /*
    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()
        { return
            new Dictionary<string, HashSet<Tuple<string, float>>>{
            { "Atacar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Manos", 0.5f),
                Tuple.Create("Azada", 0.75f),
                Tuple.Create("Lanza", 1.0f),
                Tuple.Create("Amenaza", 1.0f)
            }},
            { "Beber", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Agua", 1.0f),
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
                Tuple.Create("Carne", 0.5f),
                Tuple.Create("Agua", 0.5f),
                Tuple.Create("Baya", 1.0f)
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
                Tuple.Create("Azada", 1.0f),
                Tuple.Create("Recurso", 1.0f)
            }}
        };
    }
    */

    #region Update
    static Dictionary<string, HashSet<Tuple<string, float>>> GetGoalElementsOntology()
        { return
            new Dictionary<string, HashSet<Tuple<string, float>>>{
            { "Atacar", new HashSet<Tuple<string, float>>(){
                Tuple.Create("Manos", 0.5f),
                Tuple.Create("Azada", 0.75f),
                Tuple.Create("Lanza", 1.0f),
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
                Tuple.Create("Azada", 1.0f),
                Tuple.Create("Recurso", 1.0f)
             }}
        };
    }
    public static Dictionary<string, Data> dicGoals = new Dictionary<string, Data>(){
        { Util.StrEnum(Meta.Comer),
        new Data {  etiquetas = Util.strEnumComer[0].Split('_'),
                    prerequisitos = Util.strEnumComer[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumComer[2].Split('_')[0], Util.strEnumComer[2].Split('_')[1]),
                    rasgo = Util.strEnumComer[3]} },
        { Util.StrEnum(Meta.Beber),
        new Data {  etiquetas = Util.strEnumBeber[0].Split('_'),
                    prerequisitos = Util.strEnumBeber[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumBeber[2].Split('_')[0], Util.strEnumBeber[2].Split('_')[1]),
                    rasgo = Util.strEnumBeber[3]} },
        { Util.StrEnum(Meta.Dormir),
        new Data {  etiquetas = Util.strEnumDormir[0].Split('_'),
                    prerequisitos = Util.strEnumDormir[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumDormir[2].Split('_')[0], Util.strEnumDormir[2].Split('_')[1]),
                    rasgo = Util.strEnumDormir[3]} },
        { Util.StrEnum(Meta.Atacar),
        new Data {  etiquetas = Util.strEnumAtacar[0].Split('_'),
                    prerequisitos = Util.strEnumAtacar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumAtacar[2].Split('_')[0], Util.strEnumAtacar[2].Split('_')[1]),
                    rasgo = Util.strEnumAtacar[3]} },
        { Util.StrEnum(Meta.Huir),
        new Data {  etiquetas = Util.strEnumHuir[0].Split('_'),
                    prerequisitos = Util.strEnumHuir[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumHuir[2].Split('_')[0], Util.strEnumHuir[2].Split('_')[1]),
                    rasgo = Util.strEnumHuir[3]} },
        { Util.StrEnum(Meta.Recolectar),
        new Data {  etiquetas = Util.strEnumRecolectar[0].Split('_'),
                    prerequisitos = Util.strEnumRecolectar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumRecolectar[2].Split('_')[0], Util.strEnumRecolectar[2].Split('_')[1]),
                    rasgo = Util.strEnumRecolectar[3]} },
        { Util.StrEnum(Meta.Cocinar),
        new Data {  etiquetas = Util.strEnumCocinar[0].Split('_'),
                    prerequisitos = Util.strEnumCocinar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumCocinar[2].Split('_')[0], Util.strEnumCocinar[2].Split('_')[1]),
                    rasgo = Util.strEnumCocinar[3]} },
        { Util.StrEnum(Meta.Comerciar),
        new Data {  etiquetas = Util.strEnumComerciar[0].Split('_'),
                    prerequisitos = Util.strEnumComerciar[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumComerciar[2].Split('_')[0], Util.strEnumComerciar[2].Split('_')[1]),
                    rasgo = Util.strEnumComerciar[3]} },
        { Util.StrEnum(Meta.IrLago),
        new Data {  etiquetas = Util.strEnumIrLago[0].Split('_'),
                    prerequisitos = Util.strEnumIrLago[1].Split('_'),
                    objetivo = Tuple.Create(Util.strEnumIrLago[2].Split('_')[0], Util.strEnumIrLago[2].Split('_')[1]),
                    rasgo = Util.strEnumIrLago[3]} }
};

    #endregion Update
}
