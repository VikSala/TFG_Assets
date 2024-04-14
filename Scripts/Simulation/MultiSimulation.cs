using UnityEngine;

public class MultiSimulation : MonoBehaviour
{
    public int simulaciones = 1;
    public GameObject Simulation;
    
    [System.NonSerialized] public bool singleMultiNavMesh = false;
    [System.NonSerialized] public bool iniciarCicloDiario = false;
    
    void Start()
    {
        InstanciarSimulaciones();
    }

    void Update()
    {
        if(singleMultiNavMesh){ singleMultiNavMesh = false; iniciarCicloDiario = true; ActivarElementos(); }
    }

    void InstanciarSimulaciones()
    {
        for(int x = 0; x < simulaciones; x++)
        {
            GameObject newSimulation = Instantiate(Simulation, new Vector3(0, x*100, 0), Quaternion.identity);
            Util.seed = System.Environment.TickCount;
            newSimulation.name = "Simulation_" + Util.seed;
            newSimulation.transform.parent = gameObject.transform;
            print(newSimulation.name);
        }
        GetComponent<NavMUpdate>().doUpdate = true;
    }

    void ActivarElementos()
    {
        Util.multiSimLista = true;
        int numHijos = transform.childCount;
        
        //NavMeshValidator: i = 0
        for (int i = 1; i < numHijos; i++)
            transform.GetChild(i).GetComponent<RandomPlaneSpawner>().Elementos.SetActive(true);
    }
}
