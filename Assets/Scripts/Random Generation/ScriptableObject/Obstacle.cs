using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.RandomGeneration
{
    [CreateAssetMenu(fileName = "New Obstacle", menuName = "Scriptable Object/Random Generation/Obstacle")]
    internal sealed class Obstacle : ScriptableObject
    {
        public TilemapLayer tilemapLayer;
        public Vector2Int size;
        public byte thickness;
        public List<TileBase> tileBases;

        internal bool IsSorted { get; private set; }

        internal void Sort()
        {
            tileBases = tileBases.OrderBy(tilebase => int.Parse(tilebase.name.Substring(tilebase.name.LastIndexOf('_') + 1))).ToList();
            IsSorted = true;
        }
    }
}