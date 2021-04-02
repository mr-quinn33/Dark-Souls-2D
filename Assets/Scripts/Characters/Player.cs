using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Characters
{
    [CreateAssetMenu(fileName = "New Player", menuName = "Scriptable Object/Character/Player")]
    public sealed class Player : Character
    {
        [Header("Position")]
        [SerializeField] private Vector3 position;

        public const string tag = "Player";
        public static new event Func<float> OnDestroy;

        public sealed override async void Destroy(float time)
        {
            base.Destroy(time);
            await Task.Delay((int)(time * 1000));
            float? delay = OnDestroy?.Invoke();
            if (delay != null)
            {
                ResetStats((float)delay);
            }
        }

        public void SetPosition(Vector3 position) => this.position = position;

        public Vector3 GetPosition() => position;
    }
}