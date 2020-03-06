using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Utility
{
    public static class UiUtils
    {
        public static void RebuildParentLayoutGroups(GameObject gameObject)
        {
            foreach (var component in gameObject.ComponentsInParentQuery<HorizontalOrVerticalLayoutGroup>(true))
                LayoutRebuilder.ForceRebuildLayoutImmediate(component.GetComponent<RectTransform>());
        }

        public static void SetChildLayoutsEnabled(GameObject gameObject, bool enabled)
        {
            gameObject.SetComponentsInChildrenEnabledState<HorizontalOrVerticalLayoutGroup>(enabled);
        }

        public static void SetChildContentFittersEnabled(GameObject gameObject, bool enabled)
        {
            gameObject.SetComponentsInChildrenEnabledState<ContentSizeFitter>(enabled);
        }

        public static void UpdateAndDisableLayoutComponentsThisFrame(GameObject gameObject, bool rebuildLayoutParents = true)
        {
            SetChildLayoutsEnabled(gameObject, true);
            SetChildContentFittersEnabled(gameObject, true);

            if (rebuildLayoutParents)
                RebuildParentLayoutGroups(gameObject);

            CoroutineRunner.YieldAnCallback(null, () =>
            {
                if (gameObject == null)
                    return;

                SetChildLayoutsEnabled(gameObject, false);
                SetChildContentFittersEnabled(gameObject, false);
            });
        }

        public static void VerticalScrollNextFrame([NotNull] ScrollRect scrollRect, float value)
        {
            if (scrollRect == null) 
                throw new ArgumentNullException(nameof(scrollRect));

            CoroutineRunner.YieldAnCallback(null, () =>
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalScrollbar.value = value;
                Canvas.ForceUpdateCanvases();
            });
        }

    }
}