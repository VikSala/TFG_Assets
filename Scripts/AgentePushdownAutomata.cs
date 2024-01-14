using System.Collections.Generic;
using UnityEngine;

// Enumeración de los estados posibles del agente
public enum EstadoAgenteExistencia
{
    SinNecesidad, 
    Hambre,     //El agente tiene hambre 
    Sed,        //El agente tiene sed 
    Cansado,    //El agente tiene sueño 
    Amenaza,    //El agente detecta amenaza 
    Peligro     //El agente detecta peligro
}

public enum EstadoAgenteRealidad
{
    Recurso,    //El agente detecta un recurso
    Agente      //El agente detecta un agente
}

// Clase para manejar el estado actual y la pila de estados
public class ControladorEstados
{
    private EstadoAgenteExistencia estadoActual;
    private Stack<EstadoAgenteExistencia> pilaEstados = 
                    new Stack<EstadoAgenteExistencia>();

    public ControladorEstados()
    {
        estadoActual = EstadoAgenteExistencia.SinNecesidad;
    }

    public void CambiarEstado(EstadoAgenteExistencia nuevoEstado)
    {
        // Verificar prioridades antes de cambiar el estado
        if (VerificarPrioridad(nuevoEstado))
        {
            pilaEstados.Push(estadoActual);
            estadoActual = nuevoEstado;
        }
    }

    private bool VerificarPrioridad(EstadoAgenteExistencia nuevoEstado)
    {
        switch (nuevoEstado)
        {
            case EstadoAgenteExistencia.Hambre:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza && 
                estadoActual != EstadoAgenteExistencia.Cansado;

            case EstadoAgenteExistencia.Sed:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza && 
                estadoActual != EstadoAgenteExistencia.Cansado;

            case EstadoAgenteExistencia.Cansado:
                return estadoActual != EstadoAgenteExistencia.Peligro && 
                estadoActual != EstadoAgenteExistencia.Amenaza;

            case EstadoAgenteExistencia.Amenaza:
                return estadoActual != EstadoAgenteExistencia.Peligro;

            case EstadoAgenteExistencia.Peligro:
                return true; // Este estado tiene la máxima prioridad
                
            default:
                return true;
        }
    }

    public void FinalizarEstadoActual()
    {
        Debug.Log("Finaliza: " + estadoActual.ToString());
        // Si hay estado anterior, regresamos al estado anterior
        if (pilaEstados.Count > 0) estadoActual = pilaEstados.Pop();
    }

    public EstadoAgenteExistencia ObtenerEstadoActual()
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
            controladorEstados.CambiarEstado(EstadoAgenteExistencia.Hambre);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            controladorEstados.CambiarEstado(EstadoAgenteExistencia.Sed);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            controladorEstados.CambiarEstado(EstadoAgenteExistencia.Cansado);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            controladorEstados.CambiarEstado(EstadoAgenteExistencia.Amenaza);
            isDebug = true;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            controladorEstados.CambiarEstado(EstadoAgenteExistencia.Peligro);
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
        EstadoAgenteExistencia estadoActual = controladorEstados.ObtenerEstadoActual();
        
        switch (estadoActual)
        {
            case EstadoAgenteExistencia.SinNecesidad:
                if(isDebug) Debug.Log("El agente no tiene necesidades específicas en este momento.");
                break;
            case EstadoAgenteExistencia.Hambre:
                if(isDebug) Debug.Log("El agente tiene hambre.");
                break;
            case EstadoAgenteExistencia.Sed:
                if(isDebug) Debug.Log("El agente tiene sed.");
                break;
            case EstadoAgenteExistencia.Cansado:
                if(isDebug) Debug.Log("El agente tiene sueño.");
                break;
            case EstadoAgenteExistencia.Amenaza:
                if(isDebug) Debug.Log("El agente detecta amenaza.");
                break;
            case EstadoAgenteExistencia.Peligro:
                if(isDebug) Debug.Log("¡El agente detecta peligro!");
                break;
        }
        isDebug = false;
    }
}