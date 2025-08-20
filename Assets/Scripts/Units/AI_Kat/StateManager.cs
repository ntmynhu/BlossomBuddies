using UnityEngine;
using UnityEngine.AI;

public class StateManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;

    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;

    private BaseState currentState;
    public WalkAroundState walkAroundState = new WalkAroundState();
    public SleepingState sleepingState = new SleepingState();

    public float Energy { get; set; }
    public float Food { get; set; }
    public float Cleaness { get; set; }
    public float Happiness { get; set; }

    private void Start()
    {
        Energy = 100f;
        Food = 100f;
        Cleaness = 100f;
        Happiness = 100f;

        currentState = walkAroundState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(BaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }
}
