using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.Tilemaps;

namespace UnityEngine.AI
{
    public enum CollectObjects2D
    {
        All = 0,
        Volume = 1,
        Children = 2,
    }

    [ExecuteAlways]
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Navigation/NavMeshSurface2D", 30)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshSurface2D : MonoBehaviour
    {
        [SerializeField]
        private int m_AgentTypeID;
        public int AgentTypeID { get => m_AgentTypeID; set => m_AgentTypeID = value; }

        [SerializeField]
        private CollectObjects2D m_CollectObjects = CollectObjects2D.All;
        public CollectObjects2D CollectObjects { get => m_CollectObjects; set => m_CollectObjects = value; }

        [SerializeField]
        private Vector3 m_Size = new Vector3(10.0f, 10.0f, 10.0f);
        public Vector3 Size { get => m_Size; set => m_Size = value; }

        [SerializeField]
        private Vector3 m_Center = new Vector3(0, 2.0f, 0);
        public Vector3 Center { get => m_Center; set => m_Center = value; }

        [SerializeField]
        private LayerMask m_LayerMask = ~0;
        public LayerMask LayerMask { get => m_LayerMask; set => m_LayerMask = value; }

        [SerializeField]
        private NavMeshCollectGeometry m_UseGeometry = NavMeshCollectGeometry.RenderMeshes;
        public NavMeshCollectGeometry UseGeometry { get => m_UseGeometry; set => m_UseGeometry = value; }

        [SerializeField]
        private bool m_OverrideByGrid;
        public bool OverrideByGrid { get => m_OverrideByGrid; set => m_OverrideByGrid = value; }

        [SerializeField]
        private GameObject m_UseMeshPrefab;
        public GameObject UseMeshPrefab { get => m_UseMeshPrefab; set => m_UseMeshPrefab = value; }

        [SerializeField]
        private bool m_CompressBounds;
        public bool CompressBounds { get => m_CompressBounds; set => m_CompressBounds = value; }

        [SerializeField]
        private Vector3 m_OverrideVector = Vector3.one;
        public Vector3 OverrideVector { get => m_OverrideVector; set => m_OverrideVector = value; }

        [SerializeField]
        private int m_DefaultArea;
        public int DefaultArea { get => m_DefaultArea; set => m_DefaultArea = value; }

        [SerializeField]
        private bool m_IgnoreNavMeshAgent = true;
        public bool IgnoreNavMeshAgent { get => m_IgnoreNavMeshAgent; set => m_IgnoreNavMeshAgent = value; }

        [SerializeField]
        private bool m_IgnoreNavMeshObstacle = true;
        public bool IgnoreNavMeshObstacle { get => m_IgnoreNavMeshObstacle; set => m_IgnoreNavMeshObstacle = value; }

        [SerializeField]
        private bool m_OverrideTileSize;
        public bool OverrideTileSize { get => m_OverrideTileSize; set => m_OverrideTileSize = value; }
        [SerializeField]
        private int m_TileSize = 256;
        public int TileSize { get => m_TileSize; set => m_TileSize = value; }
        [SerializeField]
        private bool m_OverrideVoxelSize;
        public bool OverrideVoxelSize { get => m_OverrideVoxelSize; set => m_OverrideVoxelSize = value; }
        [SerializeField]
        private float m_VoxelSize;
        public float VoxelSize { get => m_VoxelSize; set => m_VoxelSize = value; }

        // Currently not supported advanced options
        [SerializeField]
        private bool m_BuildHeightMesh;
        public bool BuildHeightMesh { get => m_BuildHeightMesh; set => m_BuildHeightMesh = value; }

        // Reference to whole scene navmesh data asset.
        [UnityEngine.Serialization.FormerlySerializedAs("m_BakedNavMeshData")]
        [SerializeField]
        private NavMeshData m_NavMeshData;
        public NavMeshData NavMeshData { get => m_NavMeshData; set => m_NavMeshData = value; }

        // Do not serialize - runtime only state.
        private NavMeshDataInstance m_NavMeshDataInstance;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;

        public static List<NavMeshSurface2D> ActiveSurfaces { get; } = new List<NavMeshSurface2D>();

        private void OnEnable()
        {
            Register(this);
            AddData();
        }

        private void OnDisable()
        {
            RemoveData();
            Unregister(this);
        }

        public void AddData()
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(this);
            if (isPrefab)
            {
                //Debug.LogFormat("NavMeshData from {0}.{1} will not be added to the NavMesh world because the gameObject is a prefab.",
                //    gameObject.name, name);
                return;
            }
#endif
            if (m_NavMeshDataInstance.valid)
            {
                return;
            }

            if (m_NavMeshData != null)
            {
                m_NavMeshDataInstance = NavMesh.AddNavMeshData(m_NavMeshData, transform.position, transform.rotation);
                m_NavMeshDataInstance.owner = this;
            }

            m_LastPosition = transform.position;
            m_LastRotation = transform.rotation;
        }

        public void RemoveData()
        {
            m_NavMeshDataInstance.Remove();
            m_NavMeshDataInstance = new NavMeshDataInstance();
        }

        public NavMeshBuildSettings GetBuildSettings()
        {
            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning("No build settings for agent type ID " + AgentTypeID, this);
                buildSettings.agentTypeID = m_AgentTypeID;
            }

            if (OverrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = TileSize;
            }
            if (OverrideVoxelSize)
            {
                buildSettings.overrideVoxelSize = true;
                buildSettings.voxelSize = VoxelSize;
            }
            return buildSettings;
        }

        public void BuildNavMesh()
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            Bounds sourcesBounds = new Bounds(m_Center, Abs(m_Size));
            if (m_CollectObjects != CollectObjects2D.Volume)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            NavMeshData data = NavMeshBuilder.BuildNavMeshData(GetBuildSettings(),
                    sources, sourcesBounds, transform.position, transform.rotation);

            if (data != null)
            {
                data.name = gameObject.name;
                RemoveData();
                m_NavMeshData = data;
                if (isActiveAndEnabled)
                {
                    AddData();
                }
            }
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            List<NavMeshBuildSource> sources = CollectSources();

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            Bounds sourcesBounds = new Bounds(m_Center, Abs(m_Size));
            if (m_CollectObjects != CollectObjects2D.Volume)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            return NavMeshBuilder.UpdateNavMeshDataAsync(data, GetBuildSettings(), sources, sourcesBounds);
        }

        private static void Register(NavMeshSurface2D surface)
        {
#if UNITY_EDITOR
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(surface);
            bool isPrefab = isInPreviewScene || EditorUtility.IsPersistent(surface);
            if (isPrefab)
            {
                //Debug.LogFormat("NavMeshData from {0}.{1} will not be added to the NavMesh world because the gameObject is a prefab.",
                //    surface.gameObject.name, surface.name);
                return;
            }
#endif
            if (ActiveSurfaces.Count == 0)
            {
                NavMesh.onPreUpdate += UpdateActive;
            }

            if (!ActiveSurfaces.Contains(surface))
            {
                ActiveSurfaces.Add(surface);
            }
        }

        private static void Unregister(NavMeshSurface2D surface)
        {
            _ = ActiveSurfaces.Remove(surface);

            if (ActiveSurfaces.Count == 0)
            {
                NavMesh.onPreUpdate -= UpdateActive;
            }
        }

        private static void UpdateActive()
        {
            for (int i = 0; i < ActiveSurfaces.Count; ++i)
            {
                ActiveSurfaces[i].UpdateDataIfTransformChanged();
            }
        }

        private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
#if UNITY_EDITOR
            StageHandle myStage = StageUtility.GetStageHandle(gameObject);
            if (!myStage.IsValid())
            {
                return;
            }
#endif
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (m_CollectObjects == CollectObjects2D.Children)
            {
                modifiers = new List<NavMeshModifierVolume>(GetComponentsInChildren<NavMeshModifierVolume>());
                _ = modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifierVolume.ActiveModifiers;
            }

            foreach (NavMeshModifierVolume m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                {
                    continue;
                }

                if (!m.AffectsAgentType(m_AgentTypeID))
                {
                    continue;
                }
#if UNITY_EDITOR
                if (!myStage.Contains(m.gameObject))
                {
                    continue;
                }
#endif
                Vector3 mcenter = m.transform.TransformPoint(m.Center);
                Vector3 scale = m.transform.lossyScale;
                Vector3 msize = new Vector3(m.Size.x * Mathf.Abs(scale.x), m.Size.y * Mathf.Abs(scale.y), m.Size.z * Mathf.Abs(scale.z));

                NavMeshBuildSource src = new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.ModifierBox,
                    transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one),
                    size = msize,
                    area = m.Area
                };
                sources.Add(src);
            }
        }

        private List<NavMeshBuildSource> CollectSources()
        {
            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();

            List<NavMeshModifier> modifiers;
            if (m_CollectObjects == CollectObjects2D.Children)
            {
                modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                _ = modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
            {
                modifiers = NavMeshModifier.ActiveModifiers;
            }

            foreach (NavMeshModifier m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                {
                    continue;
                }

                if (!m.AffectsAgentType(m_AgentTypeID))
                {
                    continue;
                }

                NavMeshBuildMarkup markup = new NavMeshBuildMarkup
                {
                    root = m.transform,
                    overrideArea = m.OverrideArea,
                    area = m.Area,
                    ignoreFromBuild = m.IgnoreFromBuild
                };
                markups.Add(markup);
            }

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (m_CollectObjects == CollectObjects2D.All)
                {
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        null, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, gameObject.scene, sources);
                }
                else if (m_CollectObjects == CollectObjects2D.Children)
                {
                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        transform, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, gameObject.scene, sources);
                }
                else if (m_CollectObjects == CollectObjects2D.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(m_Center, m_Size));

                    UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                        worldBounds, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, gameObject.scene, sources);
                }
                NavMeshBuilder2DWrapper builder = new NavMeshBuilder2DWrapper
                {
                    defaultArea = DefaultArea,
                    layerMask = LayerMask,
                    agentID = AgentTypeID,
                    useMeshPrefab = UseMeshPrefab,
                    overrideByGrid = OverrideByGrid,
                    compressBounds = CompressBounds,
                    overrideVector = OverrideVector,
                    CollectGeometry = UseGeometry,
                    CollectObjects = CollectObjects,
                    parent = gameObject
                };
                NavMeshBuilder2D.CollectSources(sources, builder);

            }
            else
#endif
            {
                if (m_CollectObjects == CollectObjects2D.All)
                {
                    NavMeshBuilder.CollectSources(null, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, sources);
                }
                else if (m_CollectObjects == CollectObjects2D.Children)
                {
                    NavMeshBuilder.CollectSources(transform, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, sources);
                }
                else if (m_CollectObjects == CollectObjects2D.Volume)
                {
                    Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                    Bounds worldBounds = GetWorldBounds(localToWorld, new Bounds(m_Center, m_Size));
                    NavMeshBuilder.CollectSources(worldBounds, m_LayerMask, m_UseGeometry, m_DefaultArea, markups, sources);
                }
                NavMeshBuilder2DWrapper builder = new NavMeshBuilder2DWrapper
                {
                    defaultArea = DefaultArea,
                    layerMask = LayerMask,
                    agentID = AgentTypeID,
                    useMeshPrefab = UseMeshPrefab,
                    overrideByGrid = OverrideByGrid,
                    compressBounds = CompressBounds,
                    overrideVector = OverrideVector,
                    CollectGeometry = UseGeometry,
                    CollectObjects = CollectObjects,
                    parent = gameObject
                };
                NavMeshBuilder2D.CollectSources(sources, builder);
            }
            if (m_IgnoreNavMeshAgent)
            {
                _ = sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshAgent>() != null));
            }

            if (m_IgnoreNavMeshObstacle)
            {
                _ = sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshObstacle>() != null));
            }

            AppendModifierVolumes(ref sources);

            return sources;
        }

        private static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        public static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            Vector3 absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            Vector3 absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            Vector3 absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            Vector3 worldPosition = mat.MultiplyPoint(bounds.center);
            Vector3 worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
            return new Bounds(worldPosition, worldSize);
        }

        private Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
        {
            // Use the unscaled matrix for the NavMeshSurface
            Matrix4x4 worldToLocal = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            worldToLocal = worldToLocal.inverse;
            Bounds result = new Bounds();
            if (CollectObjects != CollectObjects2D.Children)
            {
                result.Encapsulate(CalculateGridWorldBounds(worldToLocal));
            }

            foreach (NavMeshBuildSource src in sources)
            {
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                        {
                            Mesh m = src.sourceObject as Mesh;
                            result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                            break;
                        }
                    case NavMeshBuildSourceShape.Terrain:
                        {
                            // Terrain pivot is lower/left corner - shift bounds accordingly
                            TerrainData t = src.sourceObject as TerrainData;
                            result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
                            break;
                        }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
                        break;
                }
            }
            // Inflate the bounds a bit to avoid clipping co-planar sources
            result.Expand(0.1f);
            return result;
        }

        private static Bounds CalculateGridWorldBounds(Matrix4x4 worldToLocal)
        {
            Bounds bounds = new Bounds();
            Grid grid = FindObjectOfType<Grid>();
            Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();
            if (tilemaps == null || tilemaps.Length < 1)
            {

                throw new NullReferenceException("Add at least one tilemap");
            }

            foreach (Bounds lbounds in from Tilemap tilemap in tilemaps let lbounds = GetWorldBounds(worldToLocal * tilemap.transform.localToWorldMatrix, tilemap.localBounds) select lbounds)
            {
                bounds.Encapsulate(lbounds);
            }

            bounds.Expand(0.1f);
            return bounds;
        }

        private bool HasTransformChanged() => m_LastPosition != transform.position || m_LastRotation != transform.rotation;

        private void UpdateDataIfTransformChanged()
        {
            if (HasTransformChanged())
            {
                RemoveData();
                AddData();
            }
        }

#if UNITY_EDITOR
        private bool UnshareNavMeshAsset()
        {
            // Nothing to unshare
            if (m_NavMeshData == null)
            {
                return false;
            }

            // Prefab parent owns the asset reference
            bool isInPreviewScene = EditorSceneManager.IsPreviewSceneObject(this);
            bool isPersistentObject = EditorUtility.IsPersistent(this);
            if (isInPreviewScene || isPersistentObject)
            {
                return false;
            }

            // An instance can share asset reference only with its prefab parent
            NavMeshSurface2D prefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(this);
            if (prefab != null && prefab.NavMeshData == NavMeshData)
            {
                return false;
            }

            // Don't allow referencing an asset that's assigned to another surface
            for (int i = 0; i < ActiveSurfaces.Count; ++i)
            {
                NavMeshSurface2D surface = ActiveSurfaces[i];
                if (surface != this && surface.m_NavMeshData == m_NavMeshData)
                {
                    return true;
                }
            }

            // Asset is not referenced by known surfaces
            return false;
        }

        private void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                Debug.LogWarning("Duplicating NavMeshSurface does not duplicate the referenced navmesh data", this);
                m_NavMeshData = null;
            }

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (settings.agentTypeID != -1)
            {
                // When unchecking the override control, revert to automatic value.
                const float kMinVoxelSize = 0.01f;
                if (!m_OverrideVoxelSize)
                {
                    m_VoxelSize = settings.agentRadius / 3.0f;
                }

                if (m_VoxelSize < kMinVoxelSize)
                {
                    m_VoxelSize = kMinVoxelSize;
                }

                // When unchecking the override control, revert to default value.
                const int kMinTileSize = 16;
                const int kMaxTileSize = 1024;
                const int kDefaultTileSize = 256;

                if (!m_OverrideTileSize)
                {
                    m_TileSize = kDefaultTileSize;
                }
                // Make sure tilesize is in sane range.
                if (m_TileSize < kMinTileSize)
                {
                    m_TileSize = kMinTileSize;
                }

                if (m_TileSize > kMaxTileSize)
                {
                    m_TileSize = kMaxTileSize;
                }
            }
        }
#endif
    }
}
