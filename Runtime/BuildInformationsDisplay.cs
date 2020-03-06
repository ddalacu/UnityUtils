using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

namespace Framework.Utility
{

    /// <summary>
    /// Will display data contained in <see cref="BuildInformations"/> in development builds
    /// </summary>
    [Preserve]
    public class BuildInformationsDisplay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var infos = BuildInformations.LoadDefault();

            if (infos != null)
            {
                GameObject gameObject = new GameObject(nameof(BuildInformationsDisplay));
                Object.DontDestroyOnLoad(gameObject);

                gameObject.layer = LayerMask.NameToLayer("UI");

                var canvas = gameObject.AddComponent<Canvas>();
                canvas.sortingOrder = short.MaxValue;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var child = new GameObject("DateText");
                child.transform.SetParent(gameObject.transform);
                var rectTransform = child.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.pivot = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(200, 60);

                var textComponent = child.AddComponent<Text>();
                textComponent.text = "Date:" + infos.BuildTimeAsString;
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }
    }
}

#endif
