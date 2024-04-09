using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSimulation : MonoBehaviour
{
    public int simulaciones = 1;
    public GameObject Simulation;
    
    void Start()
    {
        InstanciarSimulaciones();
    }

    void InstanciarSimulaciones()
    {
        for(int x = 0; x < simulaciones; x++)
        {
            Instantiate(Simulation, new Vector3(0, x*100, 0), Quaternion.identity);
        }
    }
}
