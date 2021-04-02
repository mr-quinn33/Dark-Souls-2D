using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class ExtensionMethods
    {
        public static IEnumerator Rotate(this Transform transform, bool isClockwise, float angle, float duration)
        {
            float deltaz, z = default, angularSpeed = angle / duration;
            if (isClockwise)
            {
                while (z > -angle)
                {
                    yield return null;
                    deltaz = angularSpeed * Time.deltaTime;
                    z -= deltaz;
                    transform.Rotate(-Vector3.forward, deltaz);
                }
            }
            else
            {
                while (z < angle)
                {
                    yield return null;
                    deltaz = angularSpeed * Time.deltaTime;
                    z += deltaz;
                    transform.Rotate(Vector3.forward, deltaz);
                }
            }
        }

        public static Transform GetTransformInChildrenWithName(this Transform root, string name)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(root);
            while (queue.Count != default)
            {
                Transform transform = queue.Dequeue();
                if (transform.name.Equals(name))
                {
                    return transform;
                }
                else
                {
                    foreach (Transform child in transform)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return null;
        }
    }

    public static class CoroutineUtility
    {
        public static IEnumerator WaitForSecondsAction(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        public static IEnumerator WaitForSecondsFunc(float seconds, Func<IEnumerator> func)
        {
            yield return new WaitForSeconds(seconds);
            yield return func();
        }

        public static IEnumerator Concat(params IEnumerator[] routines)
        {
            yield return from IEnumerator routine in routines select routine;
        }
    }

    public static class TaskUtility
    {
        public static CancellationToken RefreshTokenSource(ref CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            return cancellationTokenSource.Token;
        }
    }

    public static class SaveUtility
    {
        private const string fileName = "Save.json";
        private static readonly string path = Application.persistentDataPath;

        public static void Save(ScriptableObject scriptableObject) => File.WriteAllText(Path.Combine(path, fileName), JsonUtility.ToJson(scriptableObject, true));

        public static void Load(ScriptableObject scriptableObject)
        {
            string combinedPath = Path.Combine(path, fileName);
            if (File.Exists(combinedPath))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(combinedPath), scriptableObject);
            }
        }
    }
}