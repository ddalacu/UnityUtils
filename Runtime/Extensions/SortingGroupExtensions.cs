using System;
using UnityEngine;
using UnityEngine.Rendering;


public static class SortingGroupExtensions
{
    /// <summary>
    /// Checks if a sorting group is rendered in front of another
    /// </summary>
    /// <param name="a">the sortingGroup to check if is in front</param>
    /// <param name="b">the sortingGroup to check if is back</param>
    /// <returns></returns>
    public static bool IsInFront(this SortingGroup a, SortingGroup b)
    {
        if (a == null || b == null)
            return false;

        if (b.transform.IsChildOf(a.transform))
        {
            return false;
        }

        if (a.transform.IsChildOf(b.transform))
        {
            return true;
        }

        var groupsA = a.GetComponentsInParent<SortingGroup>();
        var groupsB = b.GetComponentsInParent<SortingGroup>();

        Array.Reverse(groupsA);
        Array.Reverse(groupsB);

        int max = Mathf.Max(groupsA.Length, groupsB.Length);

        for (int i = 0; i < max; i++)
        {
            if (i < groupsA.Length && i < groupsB.Length)//both valid
            {
                int groupsASortingOrder = groupsA[i].sortingOrder;
                int groupsBSortingOrder = groupsB[i].sortingOrder;

                if (groupsASortingOrder > groupsBSortingOrder)
                    return true;

                if (groupsASortingOrder < groupsBSortingOrder)
                    return false;

                if (groupsA[i].transform.position.z < groupsB[i].transform.position.z)
                    return true;

                if (groupsA[i].transform.position.z > groupsB[i].transform.position.z)
                    return false;

            }
            else
            {
                int aSortingOrder = a.sortingOrder;
                int bSortingOrder = b.sortingOrder;

                if (i >= groupsA.Length)
                {
                    if (aSortingOrder > groupsB[i].sortingOrder)
                    {
                        return true;
                    }

                    if (aSortingOrder < groupsB[i].sortingOrder)
                    {
                        return false;
                    }

                    return a.transform.position.z < groupsB[i].transform.position.z;
                }
                else
                {
                    if (bSortingOrder > groupsA[i].sortingOrder)
                    {
                        return false;
                    }

                    if (bSortingOrder < groupsA[i].sortingOrder)
                    {
                        return true;
                    }

                    return b.transform.position.z >= groupsA[i].transform.position.z;
                }

            }
        }

        if (a.sortingOrder > b.sortingOrder)
        {
            return true;
        }

        if (a.sortingOrder < b.sortingOrder)
        {
            return false;
        }

        return a.transform.position.z < b.transform.position.z;
    }

}
