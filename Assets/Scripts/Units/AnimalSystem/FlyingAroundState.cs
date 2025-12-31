using UnityEngine;

public class FlyingAroundState : FlyingAnimalBaseState
{
    private Vector3 startPosition;
    private float flightTimer;
    private float flightDuration; private Vector3 targetPosition;

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
            handler.ChangeState(handler.idleState);
        }
    }

    private void PickNewTarget(FlyingAnimalHandler handler)
    {
        Debug.Log(LandableRegistry.Instance.Landables.Count);
        LandableAutoRegister chosenLand = LandableRegistry.Instance.Landables[Random.Range(0, LandableRegistry.Instance.Landables.Count)];

        if (LandablePointer.TryGetRandomPointOnCollider(
            chosenLand,
            castHeight: 10f,
            maxAttempts: 20,
            obstacleMask: LayerMask.GetMask("Default"),
            clearanceRadius: 0.5f,
            out Vector3 point,
            out Vector3 normal
        ))
        {
            targetPosition = new Vector3(
                point.x,
                point.y,
                point.z
            );
        }
        else
        {
            // Nếu không tìm được điểm trên collider, thì bay lên cao một chút
            targetPosition = handler.transform.position + Vector3.up * 2f;
        }
    }

    public override void ExitState(FlyingAnimalHandler handler)
    {
    }
}
