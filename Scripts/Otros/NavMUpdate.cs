using System.Collections.Generic;
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

    public bool doUpdate = false;

    void Update() {
        if(doUpdate)
        {		//TESTING
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
            

            navBounds = GetnavBounds();
            navBounds.size = navBounds.size + boundsPadding;

            UpdateNavMesh(false);
        }
    }

    Bounds GetnavBounds()
    {
        var dungeonObjects = GetDungeonObjects();
        var bounds = new Bounds();
        bool first = true;
        foreach (var gameObject in dungeonObjects)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (first)
                {
                    bounds = renderer.bounds;
                    first = false;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }

        return bounds;
    }

    List<GameObject> GetDungeonObjects()
    {
        var result = new List<GameObject>();

        var components = GameObject.FindGameObjectsWithTag(Util.TerrainTag);
        foreach (var component in components) result.Add(component.gameObject);

        return result;
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

        var components = GameObject.FindGameObjectsWithTag(Util.TerrainTag);
        foreach (var component in components)
        {
            var gameObject = component.gameObject;
            var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter == null || meshFilter.sharedMesh == null) continue;
                NavMeshBuildSource source = CreateMeshSource(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
                meshSources.Add(source);
            }
        }
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

    void OnDrawGizmosSelected()
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
    }


    void OnDisable()
    {
        DestroyNavMesh();
    }

}