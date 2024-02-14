using UnityEngine;

public class DistanceCalculator : MonoBehaviour
{
    public Transform plano;

    void Start()
    {
        // Calcular la distancia entre los vértices opuestos del plano
        Vector3 esquina1 = plano.TransformPoint(new Vector3(-0.5f, 0f, -0.5f)); // Vértice inferior izquierdo
        Vector3 esquina2 = plano.TransformPoint(new Vector3(0.5f, 0f, 0.5f));  // Vértice superior derecho

        float distancia = Vector3.Distance(esquina1, esquina2);

        Debug.Log("La distancia entre las esquinas opuestas del plano es: " + distancia);
    }
}
