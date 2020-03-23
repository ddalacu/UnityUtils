using UnityEngine;

public static class Physics2DExtensions
{
    public static bool BoundsIsEncapsulated(Bounds Encapsulator, Bounds Encapsulating)
    {
        return Encapsulator.Contains(Encapsulating.min) && Encapsulator.Contains(Encapsulating.max);
    }

    public static bool IsInsideCollider(this Collider2D A, Collider2D B)
    {
        var bounds = B.bounds;
        var boundsA = A.bounds;

        bounds.center = new Vector3(bounds.center.x, bounds.center.y, boundsA.center.z);

        if (BoundsIsEncapsulated(bounds, boundsA) == false) return false;

        if (B is BoxCollider2D) return true;

        var distance = A.Distance(B);

        if (distance.distance >= 0) return false;

        var outside1 = distance.pointB - distance.normal * 0.004f;
        var outside2 = distance.pointA + distance.normal * 0.004f;

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(distance.pointB, outside1);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(distance.pointA, outside2);

        var a = A.OverlapPoint(outside1);
        var b = B.OverlapPoint(outside2);

        var result = A.OverlapPoint(outside1) != B.OverlapPoint(outside2);
        if (result) return true;

        if (a && b && B.OverlapPoint(outside1)) return true;

        return false;
    }

    public static bool IsColliderAInsideColliders(Collider2D A, Collider2D[] colliders)
    {
        var length = colliders.Length;
        for (var i = 0; i < length; i++)
        {
            if (colliders[i].isActiveAndEnabled == false)
                continue;

            if (A.IsInsideCollider(colliders[i]))
                return true;
        }

        return false;
    }

    public static bool AreCollidersAInsideCollidersB(Collider2D[] collidersA, Collider2D[] collidersB)
    {
        var length = collidersA.Length;
        for (var i = 0; i < length; i++)
        {
            if (collidersA[i].isActiveAndEnabled == false)
                continue;
            if (IsColliderAInsideColliders(collidersA[i], collidersB) == false) return false;
        }

        return true;
    }
}