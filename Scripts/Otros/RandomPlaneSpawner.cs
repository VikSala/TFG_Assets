using UnityEngine;
using System.Collections.Generic;

public class RandomPlaneSpawner : MonoBehaviour
{
    [SerializeField]
    public enum Escena
    {
        Reactivo,
        MultiAgente
    }
    [SerializeField] public Escena escena = Escena.MultiAgente;
    public GameObject prefabAmenaza, prefabPeligro, navigationPlane;
    public Transform planeToSpawnOn;
    
    [System.NonSerialized]
    public bool doSpawn = true;
    public List<ObjectFrequency> objetosFrecuencia = new List<ObjectFrequency>();
    int seed; bool useSeed = false; public bool esElemento = true;
    public GameObject Elementos;

    void Start()
    {
        //if(!useSeed) seed = System.Environment.TickCount;
        Random.InitState(Util.seed); //print("Seed: " + seed);//508998734//532948828//23063000
        SpawnManager();
    }

    void SpawnManager()
    {
        switch(escena)
        {
            case Escena.Reactivo: InvokeRepeating("SpawnReactivo", 0f, 1f);
                break;
            case Escena.MultiAgente: SpawnFrecuencia(); 
                break;
        }
    }

    public void SpawnFrecuencia()
    {
        int fActual, frecuencia = (int)(planeToSpawnOn.localScale.x * 5);

        GameObject respawnObject = GameObject.FindWithTag(Util.SpawnTag);
        if(respawnObject != null) Destroy(respawnObject);

        // Crear un objeto vacío como contenedor
        GameObject container = new GameObject("Contenedor");
        container.tag = Util.SpawnTag;

        // Instanciar los objetos basados en la frecuencia
        foreach (var kvp in objetosFrecuencia)
        {
            fActual = (int)(frecuencia * kvp.frequency);

            for (int i = 0; i < fActual; i++)
            {
                Vector3 spawnPosition = RandomVector();
                GameObject spawnedObject;
                // Instanciar el prefab en la posición calculada
                if(esElemento)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(spawnPosition + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.CompareTag(Util.TerrainTag))
                        {
                            // El raycast golpeó el objeto "Terrain", entonces podemos instanciar el prefab
                            spawnedObject = Instantiate(kvp.gameObject, spawnPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
                            spawnedObject.name = spawnedObject.name.Split("(Clone)")[0] + i.ToString();

                            spawnedObject.transform.parent = container.transform;
                            if(spawnedObject.name.Contains(Util.StrEnum(Lugar.Lago))) 
                                GetComponent<LugarManager>().Lugares.Add(spawnedObject);
                        }
                        else{ i--; continue;}
                    }
                }
                else
                {
                    spawnedObject = Instantiate(kvp.gameObject, spawnPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
                    spawnedObject.name = spawnedObject.name.Split("(Clone)")[0] + i.ToString();
                    spawnedObject.transform.parent = navigationPlane.transform;
                }
            }
        }
        if(esElemento) GetComponent<LugarManager>().ObtenerLugares();
        else
        {
            GetComponent<NavMUpdate>().doUpdate = true;
            Elementos.SetActive(true);
        }
    }

    void SpawnReactivo()
    {
        if(doSpawn){
            //Destroy gameobject tag Respawn
            GameObject respawnObject = GameObject.FindWithTag("Respawn");
            if(respawnObject != null) Destroy(respawnObject);
            
            //Ejecutar el spawn
            SpawnObjects();
        }
        doSpawn = false;
    }

    void SpawnObjects()
    {
        int numberOfObjects = 10;

        // Crear un objeto vacío como contenedor
        GameObject container = new GameObject("Contenedor");
        container.tag = "Respawn";

        //Instanciar: Amenazas
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 spawnPosition = RandomVector(); spawnPosition.y = 0.622f;

            // Instanciar el prefab en la posición calculada
            GameObject spawnedObject = Instantiate(prefabAmenaza, spawnPosition, Quaternion.identity);
            spawnedObject.name = Util.StrEnum(Percepcion.Amenaza);
            spawnedObject.transform.parent = container.transform;
        }

        //Instanciar: Peligro
        Vector3 enemyPosition = RandomVector();
        
        GameObject enemyObject = Instantiate(prefabPeligro, enemyPosition, Quaternion.identity);
        enemyObject.name = Util.StrEnum(Percepcion.Peligro);
        enemyObject.transform.parent = container.transform;
    }

    public Vector3 RandomVector()
    {
        float randomX, randomZ;
        // Generar posiciones aleatorias en la superficie del plano
        randomX = Random.Range(-planeToSpawnOn.localScale.x * 4.873f, planeToSpawnOn.localScale.x * 4.873f);
        randomZ = Random.Range(-planeToSpawnOn.localScale.z * 4.873f, planeToSpawnOn.localScale.z * 4.873f);
        
        return new Vector3(randomX, 0f, randomZ) + planeToSpawnOn.position;
    }

    public float GetDistanciaPlano()
    {
        // Calcular la distancia entre los vértices opuestos del plano
        Vector3 esquina1 = planeToSpawnOn.TransformPoint(new Vector3(-4.873f, 0f, -4.873f)); // Vértice inferior izquierdo
        Vector3 esquina2 = planeToSpawnOn.TransformPoint(new Vector3(4.873f, 0f, 4.873f));  // Vértice superior derecho
        
        return Vector3.Distance(esquina1, esquina2)/2;
    }
}

[System.Serializable]
public class ObjectFrequency
{
    public GameObject gameObject;

    [Range(0f, 1f)]
    public float frequency;
}