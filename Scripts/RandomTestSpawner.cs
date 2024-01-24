using UnityEngine;

public class RandomTestSpawner : MonoBehaviour
{
    public GameObject prefabAmenaza, objectPeligro;
    public Transform planeToSpawnOn;
    public int numberOfObjects = 10;
    public int seed = 123;
    public bool useSeed = false;
    
    [System.NonSerialized]
    public bool doSpawn = true;

    void Start()
    {
        InvokeRepeating("SpawnManager", 0f, 1f);
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

        float randomX, randomZ;
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generar posiciones aleatorias en la superficie del plano
            randomX = Random.Range(-planeToSpawnOn.localScale.x * 4.873f, planeToSpawnOn.localScale.x * 4.873f);
            randomZ = Random.Range(-planeToSpawnOn.localScale.z * 4.873f, planeToSpawnOn.localScale.z * 4.873f);

            // Calcular la posición final en la superficie del plano
            Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ) + planeToSpawnOn.position;

            // Instanciar el prefab en la posición calculada
            GameObject spawnedObject = Instantiate(prefabAmenaza, spawnPosition, Quaternion.identity);
            spawnedObject.transform.parent = container.transform;
        }

        //Mover Peligro
        randomX = Random.Range(-planeToSpawnOn.localScale.x * 4.873f, planeToSpawnOn.localScale.x * 4.873f);
        randomZ = Random.Range(-planeToSpawnOn.localScale.z * 4.873f, planeToSpawnOn.localScale.z * 4.873f);
        objectPeligro.transform.position = new Vector3(randomX, 1f, randomZ) + planeToSpawnOn.position;
        objectPeligro.SetActive(true);
    }
}