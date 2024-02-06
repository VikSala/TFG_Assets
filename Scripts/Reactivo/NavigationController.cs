using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigationController : MonoBehaviour
{
    public Transform targetDestination;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Verificar si el destino ha cambiado
        if (targetDestination != null && targetDestination.hasChanged)
        {
            navMeshAgent.SetDestination(targetDestination.position);
            targetDestination.hasChanged = false; // Resetear la bandera de cambio
            navMeshAgent.isStopped = false;
        }

        // Verificar la distancia al destino
        if (targetDestination != null && Vector3.Distance(transform.position, targetDestination.position) < 3f)
        {
            // Detener el agente
            navMeshAgent.isStopped = true; //print("Destino");
        }
    }
}
