using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace Assets.Scripts.RandomGeneration
{
    [RequireComponent(typeof(NavMeshSurface2D))]
    internal sealed class DungeonGenerator : MonoBehaviour
    {
        #region Field
        [Header("Dungeon")]
        [SerializeField] private ushort roomCount;
        [SerializeField] private byte roomPrefabCount;
        [SerializeField] private int seed;

        [Header("Obstacle")]
        [SerializeField] private ushort obstacleCount;
        [SerializeField] private List<Obstacle> possibleObstacles;

        [Header("Prefab")]
        [SerializeField] private ushort prefabCount;
        [SerializeField] private List<GameObject> possiblePrefabs;

        private static DungeonGenerator instance;
        private NavMeshSurface2D navMeshSurface2D;
        private Room[,] rooms;
        #endregion

        internal Room CurrentRoom { get; private set; }

        private void Awake()
        {
            if (seed != default)
            {
                Random.InitState(seed);
            }
            if (instance == null)
            {
                instance = this;
                CurrentRoom = GenerateDungeon(roomCount);
                Addressables.InstantiateAsync(CurrentRoom.PrefabName, Vector3.zero, Quaternion.identity).Completed += handle => CurrentRoom.AddPopulation(handle.Result.transform, possibleObstacles, navMeshSurface2D = GetComponent<NavMeshSurface2D>());
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Addressables.InstantiateAsync(instance.CurrentRoom.PrefabName, Vector3.zero, Quaternion.identity).Completed += handle => instance.CurrentRoom.AddPopulation(handle.Result.transform, instance.possibleObstacles, instance.navMeshSurface2D);
                Destroy(gameObject);
                Destroy(this);
            }
        }

        #region Methods
        internal void SetCurrentRoom(Room room) => CurrentRoom = room;

        private Room GenerateDungeon(ushort roomCount)
        {
            int gridSize = 3 * roomCount;
            rooms = new Room[gridSize, gridSize];
            Vector2Int initialCoordinate = new Vector2Int(gridSize >> 1, gridSize >> 1);
            Queue<Room> queue = new Queue<Room>();
            List<Room> createdRooms = new List<Room>();
            queue.Enqueue(new Room(initialCoordinate, roomPrefabCount));
            while (queue.Count > 0 && createdRooms.Count < roomCount)
            {
                Room room = queue.Dequeue();
                rooms[room.Coordinate.x, room.Coordinate.y] = room;
                createdRooms.Add(room);
                AddNeighbors(queue, room);
            }
            foreach (Room room in createdRooms)
            {
                foreach (Vector2Int coordinate in room.NeighborCoordinates)
                {
                    Room neighbor = rooms[coordinate.x, coordinate.y];
                    if (neighbor != null)
                    {
                        room.Connect(neighbor);
                    }
                }
                room.PopulateObstacles(obstacleCount, possibleObstacles);
                room.PopulatePrefabs(prefabCount, possiblePrefabs);
            }
            return rooms[initialCoordinate.x, initialCoordinate.y];
        }

        private void AddNeighbors(Queue<Room> queue, Room room)
        {
            List<Vector2Int> availableNeighbors = room.NeighborCoordinates.Where(coordinate => rooms[coordinate.x, coordinate.y] == null).ToList();
            int neighborsCount = Random.Range(1, availableNeighbors.Count);
            for (int index = 0; index < neighborsCount; index++)
            {
                float fraction, step, randomValue = Random.value;
                fraction = step = 1f / availableNeighbors.Count;
                Vector2Int neighbor = room.Coordinate;
                foreach (Vector2Int coordinate in availableNeighbors)
                {
                    if (randomValue < fraction)
                    {
                        neighbor = coordinate;
                        break;
                    }
                    else
                    {
                        fraction += step;
                    }
                }
                queue.Enqueue(new Room(neighbor, roomPrefabCount));
                _ = availableNeighbors.Remove(neighbor);
            }
        }
        #endregion
    }
}
