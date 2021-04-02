using Assets.Scripts.Characters;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.RandomGeneration
{
    internal sealed class Door : MonoBehaviour
    {
        private char direction;

        #region Callbacks
        private void Awake() => direction = gameObject.name.ToCharArray().Last();

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (collider2D.CompareTag(Player.tag))
            {
                DungeonGenerator dungeonGenerator = FindObjectOfType<DungeonGenerator>();
                if (dungeonGenerator == null)
                {
                    throw new NullReferenceException("Can not find dungeon generator in this scene!");
                }
                dungeonGenerator.SetCurrentRoom(dungeonGenerator.CurrentRoom.Neighbor(direction));
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        #endregion
    }
}