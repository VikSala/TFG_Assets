using UnityEngine;
using UnityEngine.AI;

public class CheckNavMeshValidity : MonoBehaviour
{
    public GameObject SimulationManager;
    private NavMeshAgent navMeshAgent;
    private bool navMeshValidado = false;

    void Start()
    {
        // Obtener el componente NavMeshAgent del GameObject
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Verificar si el NavMeshAgent es nulo
        if (navMeshAgent == null || SimulationManager == null)
        {
            Debug.LogError("No se encontró un componente...");
        }
    }

    void Update()
    {
        // Verificar si el NavMeshAgent está en el NavMesh y si aún no se ha validado
        if (navMeshAgent.isOnNavMesh && !navMeshValidado)
        {
            SimulationManager.GetComponent<MultiSimulation>().singleMultiNavMesh = true;//Debug.Log("¡El NavMesh es válido!");
            navMeshValidado = true;
        }
    }
}
