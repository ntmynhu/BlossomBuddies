using NUnit.Framework;
using UnityEngine;

public class FlyingAroundState : FlyingAnimalBaseState
{
    private Vector3 startPosition;
    private float flightTimer;
    private float flightDuration;
    private Vector3 targetPosition;
    private Vector3 targetRotation;

    public override void EnterState(FlyingAnimalHandler handler)
    {
        handler.Animator.Play("Fly");

        PickNewTarget(handler);

        startPosition = handler.transform.position;
        flightTimer = 0f;

        float distance = Vector3.Distance(startPosition, targetPosition);
        flightDuration = distance / handler.FlyingSpeed; // tốc độ ổn định
    }

    public override void UpdateState(FlyingAnimalHandler handler)
    {
        base.UpdateState(handler);

        flightTimer += Time.deltaTime;
        float t = Mathf.Clamp01(flightTimer / flightDuration);

        // === Di chuyển cơ bản ===
        Vector3 basePos = Vector3.Lerp(startPosition, targetPosition, t);

        // === Parabola height ===
        float arcHeight = handler.ArcHeight * flightDuration; // chỉnh độ cong
        float heightOffset = 4f * t * (1f - t) * arcHeight;

        Vector3 newPos = basePos + Vector3.up * heightOffset;
        handler.transform.position = newPos;

        // === Quay theo hướng bay ===
        Vector3 dir = (targetPosition - handler.transform.position);
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            handler.transform.rotation = Quaternion.Slerp(
                handler.transform.rotation,
                targetRot,
                Time.deltaTime * handler.RotationSpeed
            );
        }

        // === Kết thúc ===
        if (t >= 1f)
        {
            if (handler.FollowNormal)
            {
                handler.transform.rotation = Quaternion.LookRotation(-targetRotation);
            }
            else
            {
                // Giữ nguyên hướng hiện tại
                handler.transform.rotation = Quaternion.Euler(0f, handler.transform.rotation.eulerAngles.y, 0f);
            }

            handler.ChangeState(handler.idleState);
        }
    }

    private void PickNewTarget(FlyingAnimalHandler handler)
    {
        if (handler.IsFreelyLanded)
        {
            PickRandomTargetInBox(handler);
            return;
        }

        var landableList = LandableRegistry.Instance.Landables[handler.LandableType];
        if (landableList.Count == 0)
        {
            PickRandomTargetInBox(handler);
            return;
        }

        LandableAutoRegister chosenLand = landableList[Random.Range(0, landableList.Count)];

        if (LandablePointer.TryGetRandomPointOnCollider(
            chosenLand,
            castHeight: 10f,
            maxAttempts: 50,
            obstacleMask: LayerMask.GetMask("Default"),
            clearanceRadius: handler.ObstacleClearanceRadius,
            out Vector3 point,
            out Vector3 normal
        ))
        {
            targetPosition = new Vector3(
                point.x,
                point.y,
                point.z
            );

            targetRotation = normal;
        }
        else
        {
            PickRandomTargetInBox(handler);
        }
    }

    private void PickRandomTargetInBox(FlyingAnimalHandler handler)
    {
        Vector3 min = handler.FreelyLandingMin;
        Vector3 max = handler.FreelyLandingMax;

        if (LandablePointer.TryGetRandomTargetInBox(
                min,
                max,
                maxAttempts: 50,
                clearanceRadius: handler.ObstacleClearanceRadius,
                out Vector3 randomPoint
            ))
        {
            targetPosition = randomPoint;
            return;
        }
        else
        {
            // Nếu không tìm được điểm trong box, thì bay lên cao một chút
            targetPosition = handler.transform.position + Vector3.up * 2f;
            return;
        }
    }

    private void PickRandomTargetInSphere(FlyingAnimalHandler handler)
    {
        if (LandablePointer.TryGetRandomTargetInSphere(
                handler.FreelyLandingRadius,
                maxAttempts: 50,
                clearanceRadius: handler.ObstacleClearanceRadius,
                out Vector3 randompPoint
            ))
        {
            targetPosition = randompPoint;
        }
        else
        {
            // Nếu không tìm được điểm trong sphere, thì bay lên cao một chút
            targetPosition = handler.transform.position + Vector3.up * 2f;
        }
    }

    public override void ExitState(FlyingAnimalHandler handler)
    {
    }
}
