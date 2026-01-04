using TMPro;
using UnityEngine;

public static class LandablePointer
{
    public static bool TryGetRandomPointOnCollider(LandableAutoRegister land, float castHeight, int maxAttempts, LayerMask obstacleMask,
                                                    float clearanceRadius, out Vector3 point, out Vector3 normal)
    {

        Bounds b = land.Collider.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);

            if (!land.IsIgnoreRayCast)
            {
                Vector3 origin = land.IsIgnoreRayCast ? new Vector3(x, b.max.y, z) : new Vector3(x, b.max.y + castHeight, z);

                // Raycast only against THIS collider
                Ray ray = new Ray(origin, Vector3.down);
                if (!land.Collider.Raycast(ray, out RaycastHit hit, castHeight + b.size.y + 5f))
                    continue;

                Vector3 p = hit.point;

                // === Obstacle clearance check ===
                if (clearanceRadius > 0f && Physics.CheckSphere(p, clearanceRadius, obstacleMask))
                {
                    continue; // blocked → try another point
                }

                point = p;
                normal = hit.normal;
                return true;
            }
            else
            {
                point = new Vector3(x, b.max.y, z);
                normal = Vector3.up;
                return true;
            }
        }

        point = default;
        normal = default;
        return false;
    }

    public static bool TryGetRandomTargetInSphere(float radius, int maxAttempts, float clearanceRadius, out Vector3 point)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * radius;
            point = randomOffset;

            // === Obstacle clearance check ===
            if (clearanceRadius > 0f && Physics.CheckSphere(point, clearanceRadius))
            {
                continue; // blocked → try another point
            }

            return true;
        }

        point = default;
        return false;
    }
}
