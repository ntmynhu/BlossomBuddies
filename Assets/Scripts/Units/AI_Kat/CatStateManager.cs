using UnityEngine;
using UnityEngine.AI;

public class CatStateManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;

    private CatBaseState currentState;
    public CatWalkAroundState walkAroundState = new CatWalkAroundState();

    private void Start()
    {
        currentState = walkAroundState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }
}
