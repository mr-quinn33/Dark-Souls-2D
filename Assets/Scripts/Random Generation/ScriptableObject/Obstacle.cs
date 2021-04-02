using System.Collections.Generic;
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
    }
}