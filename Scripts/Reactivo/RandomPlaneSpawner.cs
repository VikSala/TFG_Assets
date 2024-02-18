using UnityEngine;
using System.Collections.Generic;

public class RandomPlaneSpawner : MonoBehaviour
{
    public GameObject prefabAmenaza, prefabPeligro;
    public Transform planeToSpawnOn;
    public int numberOfObjects = 10;
    public int seed = 123;
    public bool useSeed = false, isProtoReactivo = false;
    
    [System.NonSerialized]
    public bool doSpawn = true;

    //public Dictionary<GameObject, int> objetosFrecuencia = new Dictionary<GameObject, int>();
    public List<ObjectFrequency> objetosFrecuencia = new List<ObjectFrequency>();

    void Start()
    {
        if(isProtoReactivo) InvokeRepeating("SpawnManager", 0f, 1f);
        else SpawnPorFrecuencia();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPorFrecuencia();
        }
    }

    void SpawnPorFrecuencia()
    {
        GameObject respawnObject = GameObject.FindWithTag("Respawn");
        if(respawnObject != null) Destroy(respawnObject);

        if (!useSeed)
        {
            // Semilla basada en el tiempo actual
            seed = System.Environment.TickCount;
        }
        Random.InitState(seed);

        // Crear un objeto vacío como contenedor
        GameObject container = new GameObject("Contenedor");
        container.tag = "Respawn";

        // Instanciar los objetos basados en la frecuencia
        foreach (var kvp in objetosFrecuencia)
        {
            for (int i = 0; i < kvp.frequency; i++)
            {
                Vector3 spawnPosition = RandomVector();

                // Instanciar el prefab en la posición calculada
                GameObject spawnedObject = Instantiate(kvp.gameObject, spawnPosition, Quaternion.identity);
                spawnedObject.name = spawnedObject.name.Split("(Clone)")[0] + i.ToString();//+= i.ToString();//
                spawnedObject.transform.parent = container.transform;
            }
        }
    }

    void SpawnManager()
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
        if (!useSeed)
        {
            // Semilla basada en el tiempo actual
            seed = System.Environment.TickCount;
        }
        Random.InitState(seed);

        // Crear un objeto vacío como contenedor
        GameObject container = new GameObject("Contenedor");
        container.tag = "Respawn";

        //Instanciar: Amenazas
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 spawnPosition = RandomVector(); spawnPosition.y = 0.622f;

            // Instanciar el prefab en la posición calculada
            GameObject spawnedObject = Instantiate(prefabAmenaza, spawnPosition, Quaternion.identity);
            spawnedObject.name = "Amenaza";
            spawnedObject.transform.parent = container.transform;
        }

        //Instanciar: Peligro
        Vector3 enemyPosition = RandomVector();
        
        GameObject enemyObject = Instantiate(prefabPeligro, enemyPosition, Quaternion.identity);
        enemyObject.name = "Peligro";
        enemyObject.transform.parent = container.transform;
    }

    public Vector3 RandomVector()
    {
        float randomX, randomZ;
        // Generar posiciones aleatorias en la superficie del plano
        randomX = Random.Range(-planeToSpawnOn.localScale.x * 4.873f, planeToSpawnOn.localScale.x * 4.873f);
        randomZ = Random.Range(-planeToSpawnOn.localScale.z * 4.873f, planeToSpawnOn.localScale.z * 4.873f);
        
        return new Vector3(randomX, 1f, randomZ) + planeToSpawnOn.position;
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
    public int frequency;
}