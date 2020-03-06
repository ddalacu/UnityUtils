using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Utility
{
    ///<summary>
    /// This class is used to run coroutines without needing to inherit from MonoBehaviour
    /// Author Paul Diac
    /// Version 0.0a
    ///</summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        private static CoroutineRunner LazyInstance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject runnerObject = new GameObject(nameof(CoroutineRunner));
                    _instance = runnerObject.AddComponent<CoroutineRunner>();
                    runnerObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
                    DontDestroyOnLoad(runnerObject);
                }

                return _instance;
            }
        }

        public static Coroutine StartCoroutineStatic(IEnumerator routine)
        {
            return LazyInstance.StartCoroutine(routine);
        }


        public static void StopCoroutineStatic(Coroutine routine)
        {
            if (_instance == null)
            {
                return;
            }

            _instance.StopCoroutine(routine);
        }

        public static Coroutine YieldAnCallback(object toYield, [NotNull] Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return LazyInstance.StartCoroutine(GeneralCoroutine(toYield, action));
        }

        private static IEnumerator GeneralCoroutine(object toYield, Action action)
        {
            yield return toYield;
            action?.Invoke();
        }

    }
}
