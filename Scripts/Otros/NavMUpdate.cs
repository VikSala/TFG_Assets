using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class NavMUpdate : MonoBehaviour
{
    /// <summary>
    /// Should dynamic navigation be created during runtime (For NPC AI)
    /// </summary>
    public bool enableRuntimeNavigation = true;

    Vector3 boundsPadding = Vector3.zero;

    // The size of the build bounds
    Bounds navBounds;
    NavMeshData m_NavMesh;
    NavMeshDataInstance m_Instance;
    List<NavMeshBuildSource> meshSources = new List<NavMeshBuildSource>();

    public bool doUpdate = false, multiSimulation = false;

    void Update() {
        if(doUpdate)
        {		
            BuildNavMesh();
            doUpdate = false;
        }
    }

    public void BuildNavMesh()
    {
        DestroyNavMesh();

        if (enableRuntimeNavigation && Application.isPlaying)
        {
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
            

            navBounds = GetNavBounds();
            navBounds.size = navBounds.size + boundsPadding;

            UpdateNavMesh(multiSimulation);
        }
    }

    Bounds GetNavBounds()
    {
        List<GameObject> navObjects = GetNavObjects();
        return GetBoundsRecursive(navObjects, new Bounds(), true);
    }

    Bounds GetBoundsRecursive(List<GameObject> navObjects, Bounds bounds, bool first)
    {
        if (navObjects.Count == 0)
        {
            return bounds;
        }

        GameObject gameObject = navObjects[0];
        navObjects.RemoveAt(0);

        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        bounds = UpdateBounds(renderers, bounds, first);

        return GetBoundsRecursive(navObjects, bounds, false);
    }

    Bounds UpdateBounds(Renderer[] renderers, Bounds bounds, bool first)
    {
        if (renderers.Length == 0)
        {
            return bounds;
        }

        Renderer renderer = renderers[0];
        bounds = UpdateBounds(renderers.Skip(1).ToArray(), bounds, first);

        if (first)
        {
            bounds = renderer.bounds;
            first = false;
        }
        else
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    List<GameObject> GetNavObjects()
    {
        return GetNavObjectsRecursive(GameObject.FindGameObjectsWithTag(Util.TerrainTag), new List<GameObject>());
    }

    List<GameObject> GetNavObjectsRecursive(GameObject[] components, List<GameObject> result)
    {
        if (components.Length == 0)
        {
            return result;
        }

        GameObject component = components[0];
        result.Add(component.gameObject);

        return GetNavObjectsRecursive(components.Skip(1).ToArray(), result);
    }

    public void DestroyNavMesh()
    {
        // Unload navmesh and clear handle
        m_Instance.Remove();
        m_NavMesh = null;
        meshSources.Clear();

        navBounds = new Bounds();
    }

    void CollectMeshSources()
    {
        meshSources.Clear();
        CollectMeshSourcesRecursive(GameObject.FindGameObjectsWithTag(Util.TerrainTag), 0);
    }

    void CollectMeshSourcesRecursive(GameObject[] components, int index)
    {
        if (index >= components.Length)
        {
            return;
        }

        GameObject gameObject = components[index];
        ProcessMeshFilters(gameObject.GetComponentsInChildren<MeshFilter>(), 0);
        CollectMeshSourcesRecursive(components, index + 1);
    }

    void ProcessMeshFilters(MeshFilter[] meshFilters, int index)
    {
        if (index >= meshFilters.Length)
        {
            return;
        }

        MeshFilter meshFilter = meshFilters[index];
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            NavMeshBuildSource source = CreateMeshSource(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
            meshSources.Add(source);
        }

        ProcessMeshFilters(meshFilters, index + 1);
    }

    NavMeshBuildSource CreateMeshSource(Mesh mesh, Matrix4x4 transform)
    {
        var source = new NavMeshBuildSource();
        source.shape = NavMeshBuildSourceShape.Mesh;
        source.sourceObject = mesh;
        source.transform = transform;
        source.area = 0;
        return source;
    }

    void UpdateNavMesh(bool asyncUpdate = false)
    {
        CollectMeshSources();


        //NavMeshSourceTag.Collect(ref m_Sources);
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);

        if (asyncUpdate)
        {
            /*m_Operation = */
            NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, meshSources, navBounds);
        }
        else
        {
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, meshSources, navBounds);
        }
    }

    /*void OnDrawGizmosSelected()
    {
        if (!enableRuntimeNavigation) return;

        if (m_NavMesh)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(m_NavMesh.sourceBounds.center, m_NavMesh.sourceBounds.size);
        }

        Gizmos.color = Color.green;
        var center = navBounds.center;
        var size = navBounds.size;
        Gizmos.DrawWireCube(center, size);
    }*/


    void OnDisable()
    {
        DestroyNavMesh();
    }

}