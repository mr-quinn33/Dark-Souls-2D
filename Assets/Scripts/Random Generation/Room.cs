using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.RandomGeneration
{
    internal sealed class Room
    {
        #region Fields
        private int index;
        private const byte maxSearchDepth = 32;
        private const byte doorWidth = 3;
        private const ushort xTileCount = 27;
        private const ushort yTileCount = 15;
        private const string occupied = "Occupied";
        private const string nontraversable = "Nontraversable";
        private const string traversable = "Traversable";
        private static readonly Vector2Int prefabSize = new Vector2Int(3, 4);
        private List<string> prefabNames;
        private Dictionary<(int x, int y), string> population;
        private Dictionary<char, Room> neighbors;
        private Dictionary<string, GameObject> namePrefabPairs;
        #endregion

        #region Properties
        internal Vector2Int Coordinate { get; private set; }

        internal List<Vector2Int> NeighborCoordinates => new List<Vector2Int>
        {
            new Vector2Int(Coordinate.x, Coordinate.y - 1),
            new Vector2Int(Coordinate.x, Coordinate.y + 1),
            new Vector2Int(Coordinate.x - 1, Coordinate.y),
            new Vector2Int(Coordinate.x + 1, Coordinate.y)
        };

        internal string PrefabName
        {
            get
            {
                string room = "Room_";
                StringBuilder stringBuilder = new StringBuilder(room);
                foreach (KeyValuePair<char, Room> keyValuePair in neighbors)
                {
                    stringBuilder = stringBuilder.Append(keyValuePair.Key);
                }
                string result = stringBuilder.ToString();
                return result.Equals(room) ? $"{room}{index}" : $"{result}_{index}";
            }
        }

        private short XLowerBound => (-xTileCount >> 1) + 3;

        private short XUpperBound => (xTileCount >> 1) + 1;

        private short YLowerBound => -yTileCount >> 1;

        private short YUpperBound => yTileCount >> 1;
        #endregion

        internal Room(Vector2Int coordinate, byte roomPrefabCount)
        {
            index = UnityEngine.Random.Range(default, roomPrefabCount);
            Coordinate = coordinate;
            neighbors = new Dictionary<char, Room>();
            population = new Dictionary<(int x, int y), string>();
            for (int i = XLowerBound; i < XUpperBound; i++)
            {
                for (int j = YLowerBound; j < YUpperBound; j++)
                {
                    population.Add((i, j), string.Empty);
                    if (-doorWidth >> 1 <= i && i <= (doorWidth >> 1) + 1)
                    {
                        population[(i, j)] = occupied;
                        continue;
                    }
                    if (-doorWidth >> 1 <= j && j <= doorWidth >> 1)
                    {
                        population[(i, j)] = occupied;
                        continue;
                    }
                }
            }
            namePrefabPairs = new Dictionary<string, GameObject>();
            prefabNames = new List<string>();
        }

        #region Methods
        internal Room Neighbor(char direction) => neighbors[direction] ?? throw new ArgumentOutOfRangeException(nameof(direction), $"{direction} does not exist!");

        internal void Connect(Room neighbor)
        {
            if (neighbor.Coordinate.y < Coordinate.y)
            {
                neighbors.Add('U', neighbor);
                return;
            }
            if (neighbor.Coordinate.y > Coordinate.y)
            {
                neighbors.Add('D', neighbor);
                return;
            }
            if (neighbor.Coordinate.x < Coordinate.x)
            {
                neighbors.Add('L', neighbor);
                return;
            }
            if (neighbor.Coordinate.x > Coordinate.x)
            {
                neighbors.Add('R', neighbor);
                return;
            }
        }

        internal void PopulateObstacles(int obstacleCount, List<Obstacle> possibleObstacles)
        {
            for (int i = 0; i < obstacleCount; i++)
            {
                Obstacle obstacle = possibleObstacles[UnityEngine.Random.Range(default, possibleObstacles.Count)];
                foreach (Vector2Int coordinate in FindFreeRegion(obstacle.size))
                {
                    population[(coordinate.x, coordinate.y)] = obstacle.name;
                }
            }
        }

        internal void PopulatePrefabs(int prefabCount, List<GameObject> possiblePrefabs)
        {
            for (int i = 0; i < prefabCount; i++)
            {
                GameObject prefab = possiblePrefabs[UnityEngine.Random.Range(default, possiblePrefabs.Count)];
                List<Vector2Int> region = FindFreeRegion(prefabSize);
                if (region.Count != default)
                {
                    prefabNames.Add(population[(region[default].x, region[default].y)] = prefab.name);
                    namePrefabPairs[prefab.name] = prefab;
                }
            }
        }

        internal void AddPopulation(Transform transform, List<Obstacle> possibleObstacles, NavMeshSurface2D navMeshSurface2D)
        {
            Dictionary<(int x, int y), string> population = new Dictionary<(int x, int y), string>(this.population);
            for (int i = XLowerBound; i < XUpperBound; i++)
            {
                for (int j = YLowerBound; j < YUpperBound; j++)
                {
                    foreach (Obstacle obstacle in possibleObstacles.Where(obstacle => population[(i, j)].Equals(obstacle.name)))
                    {
                        int xmax = i + obstacle.size.x, ymax = j + obstacle.size.y;
                        LinkedList<Vector3Int> positions = new LinkedList<Vector3Int>();
                        if (!obstacle.IsSorted)
                        {
                            obstacle.Sort();
                        }
                        for (int y = j; y < ymax; y++)
                        {
                            for (int x = xmax - 1; x >= i; x--)
                            {
                                positions.AddFirst(new LinkedListNode<Vector3Int>(new Vector3Int(x, y, default)));
                                population[(x, y)] = occupied;
                            }
                        }
                        if (obstacle.tilemapLayer.name.Equals(nontraversable))
                        {
                            Transform traversableTransform = transform.GetTransformInChildrenWithName(traversable);
                            GameObject traversableObject = UnityEngine.Object.Instantiate(traversableTransform.gameObject, Vector3.zero, Quaternion.identity, traversableTransform.parent);
                            traversableObject.GetComponent<Tilemap>().SetTiles(positions.ToArray(), obstacle.tileBases.ToArray());
                            Transform nontraversableTransform = transform.GetTransformInChildrenWithName(nontraversable);
                            GameObject nontraversableObject = UnityEngine.Object.Instantiate(nontraversableTransform.gameObject, Vector3.zero, Quaternion.identity, nontraversableTransform.parent);
                            nontraversableObject.GetComponent<Tilemap>().SetTiles(positions.Skip(obstacle.size.x * (obstacle.size.y - obstacle.thickness)).ToArray(), obstacle.tileBases.Skip(obstacle.size.x * (obstacle.size.y - obstacle.thickness)).ToArray());
                            navMeshSurface2D.BuildNavMesh();
                        }
                        else
                        {
                            Transform tilemapTransform = transform.GetTransformInChildrenWithName(obstacle.tilemapLayer.name);
                            if (tilemapTransform && tilemapTransform.TryGetComponent(out Tilemap tilemap))
                            {
                                tilemap.SetTiles(positions.ToArray(), obstacle.tileBases.ToArray());
                            }
                        }
                        goto next;
                    }
                    if (prefabNames.Contains(population[(i, j)]))
                    {
                        _ = UnityEngine.Object.Instantiate(namePrefabPairs[population[(i, j)]], new Vector2(i, j), Quaternion.identity);
                        int xmax = i + (prefabSize.x >> 1), ymax = j + (prefabSize.y >> 1);
                        for (int x = i - (prefabSize.x >> 1); x < xmax; x++)
                        {
                            for (int y = j - (prefabSize.y >> 1); y < ymax; y++)
                            {
                                population[(x, y)] = occupied;
                            }
                        }
                    }
                next:;
                }
            }
        }

        private List<Vector2Int> FindFreeRegion(Vector2Int size)
        {
            byte count = default;
            List<Vector2Int> region = new List<Vector2Int>();
            do
            {
                region.Clear();
                if (count == maxSearchDepth)
                {
                    return region;
                }
                count++;
                Vector2Int centerTile = new Vector2Int(UnityEngine.Random.Range(XLowerBound, XUpperBound), UnityEngine.Random.Range(YLowerBound, YUpperBound));
                region.Add(centerTile);
                int x = centerTile.x - (size.x >> 1), tmax = x + size.x, y = centerTile.y - (size.y >> 1), kmax = y + size.y;
                for (int t = x; t < tmax; t++)
                {
                    for (int k = y; k < kmax; k++)
                    {
                        region.Add(new Vector2Int(t, k));
                    }
                }
            } while (!IsFree(region));
            return region;
        }

        private bool IsFree(List<Vector2Int> region)
        {
            foreach (Vector2Int tile in region)
            {
                if (population.ContainsKey((tile.x, tile.y)) && !population[(tile.x, tile.y)].Equals(string.Empty))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion Methods
    }
}