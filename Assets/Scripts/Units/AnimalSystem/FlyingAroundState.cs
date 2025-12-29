using UnityEngine;

public class FlyingAroundState : FlyingAnimalBaseState
{
    private Vector3 targetPosition;

    public override void EnterState(FlyingAnimalHandler handler)
    {
        handler.Animator.Play("Fly");

        targetPosition = handler.Animator.transform.position;
        targetPosition.y = Random.Range(handler.MinHeight, handler.MaxHeight);
    }

    public override void UpdateState(FlyingAnimalHandler handler)
    {
        base.UpdateState(handler);

        handler.transform.position = Vector3.MoveTowards(handler.transform.position, targetPosition, handler.FlyingSpeed * Time.deltaTime);
    }

    public override void ExitState(FlyingAnimalHandler handler)
    {
        throw new System.NotImplementedException();
    }
}
