using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine.AI
{
    internal class NavMeshBuilder2DWrapper
    {
        public Dictionary<Sprite, Mesh> map;
        public Dictionary<uint, Mesh> coliderMap;
        public int defaultArea;
        public int layerMask;
        public int agentID;
        public bool overrideByGrid;
        public GameObject useMeshPrefab;
        public bool compressBounds;
        public Vector3 overrideVector;
        public NavMeshCollectGeometry CollectGeometry;
        public CollectObjects2D CollectObjects;
        public GameObject parent;

        public NavMeshBuilder2DWrapper()
        {
            map = new Dictionary<Sprite, Mesh>();
            coliderMap = new Dictionary<uint, Mesh>();
        }

        public Mesh GetMesh(Sprite sprite)
        {
            Mesh mesh;
            if (map.ContainsKey(sprite))
            {
                mesh = map[sprite];
            }
            else
            {
                mesh = new Mesh();
                NavMeshBuilder2D.SpriteToMesh(sprite, mesh);
                map.Add(sprite, mesh);
            }
            return mesh;
        }

        internal Mesh GetMesh(Collider2D collider)
        {
#if UNITY_2019_3_OR_NEWER
            Mesh mesh;
            uint hash = collider.GetShapeHash();
            if (coliderMap.ContainsKey(hash))
            {
                mesh = coliderMap[hash];
            }
            else
            {
                mesh = collider.CreateMesh(false, false);
                coliderMap.Add(hash, mesh);
            }
            return mesh;
#else
            throw new InvalidOperationException("PhysicsColliders supported in Unity 2019.3 and higher.");
#endif
        }

        internal IEnumerable<GameObject> GetRoot()
        {
            switch (CollectObjects)
            {
                case CollectObjects2D.Children: return new[] { parent };
                case CollectObjects2D.Volume:
                case CollectObjects2D.All:
                default:
                    return new[] { Object.FindObjectOfType<Grid>().gameObject };
            }
        }
    }

    internal class NavMeshBuilder2D
    {
        internal static void CollectSources(List<NavMeshBuildSource> sources, NavMeshBuilder2DWrapper builder)
        {
            IEnumerable<GameObject> root = builder.GetRoot();
            foreach (GameObject it in root)
            {
                CollectSources(it, sources, builder);
            }
        }

        private static void CollectSources(GameObject root, List<NavMeshBuildSource> sources, NavMeshBuilder2DWrapper builder)
        {
            foreach (NavMeshModifier modifier in root.GetComponentsInChildren<NavMeshModifier>())
            {
                if (((0x1 << modifier.gameObject.layer) & builder.layerMask) == 0)
                {
                    continue;
                }
                if (!modifier.AffectsAgentType(builder.agentID))
                {
                    continue;
                }
                int area = builder.defaultArea;
                //if it is walkable
                if (builder.defaultArea != 1 && !modifier.IgnoreFromBuild)
                {
                    Tilemap tilemap = modifier.GetComponent<Tilemap>();
                    if (tilemap != null)
                    {
                        if (builder.compressBounds)
                        {
                            tilemap.CompressBounds();
                        }

                        Debug.Log($"Walkable Bounds [{tilemap.name}]: {tilemap.localBounds}");
                        NavMeshBuildSource box = BoxBoundSource(NavMeshSurface2D.GetWorldBounds(tilemap.transform.localToWorldMatrix, tilemap.localBounds));
                        box.area = builder.defaultArea;
                        sources.Add(box);
                    }
                }

                if (modifier.OverrideArea)
                {
                    area = modifier.Area;
                }
                if (!modifier.IgnoreFromBuild)
                {
                    if (builder.CollectGeometry == NavMeshCollectGeometry.PhysicsColliders)
                    {
                        CollectSources(sources, modifier, area, builder);
                    }
                    else
                    {
                        Tilemap tilemap = modifier.GetComponent<Tilemap>();
                        if (tilemap != null)
                        {
                            CollectTileSources(sources, tilemap, area, builder);
                        }
                        SpriteRenderer sprite = modifier.GetComponent<SpriteRenderer>();
                        if (sprite != null)
                        {
                            CollectSources(sources, sprite, area, builder);
                        }
                    }
                }
            }
            //Debug.Log("Sources " + sources.Count);
        }

        private static void CollectSources(List<NavMeshBuildSource> sources, SpriteRenderer sprite, int area, NavMeshBuilder2DWrapper builder)
        {
            if (sprite == null)
            {
                return;
            }
            NavMeshBuildSource src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                area = area
            };

            Mesh mesh;
            mesh = builder.GetMesh(sprite.sprite);
            if (mesh == null)
            {
                Debug.Log($"{sprite.name} mesh is null");
                return;
            }
            src.transform = Matrix4x4.TRS(Vector3.Scale(sprite.transform.position, builder.overrideVector), sprite.transform.rotation, sprite.transform.lossyScale);
            src.sourceObject = mesh;
            sources.Add(src);
        }

        private static void CollectSources(List<NavMeshBuildSource> sources, NavMeshModifier modifier, int area, NavMeshBuilder2DWrapper builder)
        {
            Collider2D collider = modifier.GetComponent<Collider2D>();
            if (collider == null)
            {
                return;
            }

            if (collider.usedByComposite)
            {
                collider = collider.GetComponent<CompositeCollider2D>();
            }

            NavMeshBuildSource src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                area = area
            };

            Mesh mesh;
            mesh = builder.GetMesh(collider);
            if (mesh == null)
            {
                Debug.Log($"{collider.name} mesh is null");
                return;
            }
            src.transform = collider.attachedRigidbody
                ? Matrix4x4.TRS(Vector3.Scale(collider.transform.position, builder.overrideVector), collider.transform.rotation, Vector3.one)
                : Matrix4x4.identity;
            src.sourceObject = mesh;
            sources.Add(src);
        }

        private static void CollectTileSources(List<NavMeshBuildSource> sources, Tilemap tilemap, int area, NavMeshBuilder2DWrapper builder)
        {
            BoundsInt bound = tilemap.cellBounds;

            Vector3Int vec3int = new Vector3Int(0, 0, 0);

            Vector3 size = new Vector3(tilemap.layoutGrid.cellSize.x, tilemap.layoutGrid.cellSize.y, 0);
            Mesh sharedMesh = null;
            Quaternion rot = default;

            NavMeshBuildSource src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                area = area
            };

            Mesh mesh;

            if (builder.useMeshPrefab != null)
            {
                sharedMesh = builder.useMeshPrefab.GetComponent<MeshFilter>().sharedMesh;
                size = builder.useMeshPrefab.transform.localScale;
                rot = builder.useMeshPrefab.transform.rotation;
            }
            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                for (int j = bound.yMin; j < bound.yMax; j++)
                {
                    vec3int.x = i;
                    vec3int.y = j;
                    if (!tilemap.HasTile(vec3int))
                    {
                        continue;
                    }

                    if (!builder.overrideByGrid && tilemap.GetColliderType(vec3int) == Tile.ColliderType.Sprite)
                    {
                        mesh = builder.GetMesh(tilemap.GetSprite(vec3int));
                        src.transform = Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), builder.overrideVector) - tilemap.layoutGrid.cellGap, tilemap.transform.rotation, tilemap.transform.lossyScale) * tilemap.orientationMatrix * tilemap.GetTransformMatrix(vec3int);
                        src.sourceObject = mesh;
                        sources.Add(src);
                    }
                    else if (builder.useMeshPrefab != null || (builder.overrideByGrid && builder.useMeshPrefab != null))
                    {
                        src.transform = Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), builder.overrideVector), rot, size);
                        src.sourceObject = sharedMesh;
                        sources.Add(src);
                    }
                    else //default to box
                    {
                        sources.Add(new NavMeshBuildSource
                        {
                            transform = Matrix4x4.TRS(Vector3.Scale(tilemap.GetCellCenterWorld(vec3int), builder.overrideVector) - tilemap.layoutGrid.cellGap, tilemap.transform.rotation, tilemap.transform.lossyScale) * tilemap.orientationMatrix * tilemap.GetTransformMatrix(vec3int),
                            shape = NavMeshBuildSourceShape.Box,
                            size = size,
                            area = area
                        });
                    }
                }
            }
        }

        internal static void SpriteToMesh(Sprite sprite, Mesh mesh)
        {
            Vector3[] vert = new Vector3[sprite.vertices.Length];
            for (int i = 0; i < sprite.vertices.Length; i++)
            {
                vert[i] = new Vector3(sprite.vertices[i].x, sprite.vertices[i].y, 0);
            }
            mesh.vertices = vert;
            mesh.uv = sprite.uv;
            int[] tri = new int[sprite.triangles.Length];
            for (int i = 0; i < sprite.triangles.Length; i++)
            {
                tri[i] = sprite.triangles[i];
            }
            mesh.triangles = tri;
        }

        private static NavMeshBuildSource BoxBoundSource(Bounds localBounds) => new NavMeshBuildSource
        {
            transform = Matrix4x4.Translate(localBounds.center),
            shape = NavMeshBuildSourceShape.Box,
            size = localBounds.size,
            area = 0
        };
    }
}
