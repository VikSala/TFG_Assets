using UnityEngine;

public class DestruirAlEntrar : MonoBehaviour
{
    public bool toDestroy = false;
    string strName;

    void Awake(){strName = gameObject.name;}

    void OnTriggerStay(Collider other)//OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            // Destruir este objeto
            if(toDestroy)
            {   
                strName = gameObject.name;
                if(!strName.Contains(Util.StrEnum(Percepcion.Amenaza)+"_")) Destroy(gameObject);
                else 
                    gameObject.name = Util.StrEnum(Percepcion.Recurso) + 
                                      "_" + Util.StrEnum(Objeto.Carne) + 
                                      strName.Substring(strName.IndexOf("_"));
            }
        }
    }
}
