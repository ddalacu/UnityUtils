using UnityEngine.UIElements;

public static class UiElementsExtensions
{
    /// <summary>
    /// Tries to return the last child of the <see cref="VisualElement"/>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    public static bool GetLast(this VisualElement element, out VisualElement last)
    {
        var childCount = element.childCount;
        if (childCount == 0)
        {
            last = default;
            return false;
        }

        last = element[childCount - 1];
        return true;
    }

    /// <summary>
    /// Tries to remove the last child of the <see cref="VisualElement"/>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    public static bool RemoveLast(this VisualElement element, out VisualElement last)
    {
        var childCount = element.childCount;
        if (childCount == 0)
        {
            last = default;
            return false;
        }

        var lastIndex = childCount - 1;
        last = element[lastIndex];
        element.RemoveAt(lastIndex);
        return true;
    }

    /// <summary>
    /// Tries to return the last child of the <see cref="VisualElement"/> of type <see cref="T"/>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    public static bool GetLast<T>(this VisualElement element, out T last) where T : VisualElement
    {
        for (int i = element.childCount - 1; i >= 0; i--)
        {
            if (element[i] is T result)
            {
                last = result;
                return true;
            }
        }

        last = default;
        return false;
    }

    /// <summary>
    /// Tries to remove the last child of the <see cref="VisualElement"/> of type <see cref="T"/>
    /// </summary>
    /// <param name="element"></param>
    /// <param name="last"></param>
    /// <returns></returns>
    public static bool RemoveLast<T>(this VisualElement element, out T last) where T : VisualElement
    {
        for (int i = element.childCount - 1; i >= 0; i--)
        {
            if (element[i] is T result)
            {
                element.RemoveAt(i);

                last = result;
                return true;
            }
        }

        last = default;
        return false;
    }

    /// <summary>
    /// Tells if the element have any children
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HaveChildrens(this VisualElement element)
    {
        return element.childCount != 0;
    }
}