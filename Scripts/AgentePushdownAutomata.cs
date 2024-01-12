using System.Collections.Generic;
using UnityEngine;

// Enumeración de los estados posibles del agente
public enum EstadoAgente
{
    SinNecesidad,
    Hambre,
    Sed,
    HuirPeligro,
    // Otros estados...
}

// Clase para manejar el estado actual y la pila de estados
public class ControladorEstados
{
    private EstadoAgente estadoActual;
    private Stack<EstadoAgente> pilaEstados = new Stack<EstadoAgente>();

    public ControladorEstados()
    {
        estadoActual = EstadoAgente.SinNecesidad;
    }

    public void CambiarEstado(EstadoAgente nuevoEstado)
    {
        // Verificar prioridades antes de cambiar el estado
        if (VerificarPrioridad(nuevoEstado))
        {
            pilaEstados.Push(estadoActual);
            estadoActual = nuevoEstado;
        }
    }

    public void FinalizarEstadoActual()
    {
        // Puedes agregar lógica adicional si es necesario al finalizar un estado
        Debug.Log("Finaliza: " + estadoActual.ToString());
        // Por ahora, simplemente regresamos al estado anterior
        RegresarEstadoAnterior();
    }

    private bool VerificarPrioridad(EstadoAgente nuevoEstado)
    {
        // Verificar prioridades antes de cambiar el estado
        switch (nuevoEstado)
        {
            case EstadoAgente.Hambre:
                return estadoActual != EstadoAgente.HuirPeligro && estadoActual != EstadoAgente.Sed;
            case EstadoAgente.Sed:
                return estadoActual != EstadoAgente.HuirPeligro;
            case EstadoAgente.HuirPeligro:
                return true; // Este estado tiene la máxima prioridad
            default:
                return true;
        }
    }

    public void RegresarEstadoAnterior()
    {
        if (pilaEstados.Count > 0)
        {
            estadoActual = pilaEstados.Pop();
        }
    }

    public EstadoAgente ObtenerEstadoActual()
    {
        return estadoActual;
    }
}

// Clase principal del agente
public class AgentePushdownAutomata : MonoBehaviour
{
    private ControladorEstados controladorEstados;
    bool isDebug = false;

    private void Start()
    {
        controladorEstados = new ControladorEstados();
    }

    private void Update()
    {
        // Simular cambios en el estado del agente desde el editor de Unity
        if (Input.GetKeyDown(KeyCode.Q))
        {
            controladorEstados.CambiarEstado(EstadoAgente.Hambre);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            controladorEstados.CambiarEstado(EstadoAgente.Sed);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            controladorEstados.CambiarEstado(EstadoAgente.HuirPeligro);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            controladorEstados.FinalizarEstadoActual();
        }

        // Tomar decisiones basadas en el estado actual
        TomarDecisiones();
    }

    private void TomarDecisiones()
    {
        EstadoAgente estadoActual = controladorEstados.ObtenerEstadoActual();

        // Tomar decisiones según el estado actual
        switch (estadoActual)
        {
            case EstadoAgente.SinNecesidad:
                if(isDebug) Debug.Log("El agente no tiene necesidades específicas en este momento.");
                break;
            case EstadoAgente.Hambre:
                if(isDebug) Debug.Log("El agente tiene hambre. Comer algo.");
                break;
            case EstadoAgente.Sed:
                if(isDebug) Debug.Log("El agente tiene sed. Beber agua.");
                break;
            case EstadoAgente.HuirPeligro:
                if(isDebug) Debug.Log("¡Peligro! El agente está huyendo.");
                break;
        }
        isDebug = false;
    }
}