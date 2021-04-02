using Assets.Scripts.Characters;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public sealed class GameManager : MonoBehaviour
    {
        public float sceneLoadDelay;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(Instance.gameObject);
                Destroy(Instance);
            }
            DontDestroyOnLoad((Instance = this).gameObject);
        }

        private void OnEnable() => Player.OnDestroy += ReloadScene;

        private void OnDisable() => Player.OnDestroy -= ReloadScene;

        private float ReloadScene()
        {
            _ = StartCoroutine(CoroutineUtility.WaitForSecondsAction(sceneLoadDelay, () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)));
            return sceneLoadDelay;
        }
    }
}