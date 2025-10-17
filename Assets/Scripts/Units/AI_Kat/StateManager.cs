using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StateManager : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Rigidbody rb;

    #region Properties
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    #endregion

    private BaseState currentState;
    public WalkAroundState walkAroundState = new WalkAroundState();
    public SleepingState sleepingState = new SleepingState();
    public EatingState eatingState = new EatingState();
    public ChasingPlayer chasingPlayerState = new ChasingPlayer();
    public RunAwayFromPlayer runAwayFromPlayerState = new RunAwayFromPlayer();
    public BeingPickUp beingPickUpState = new BeingPickUp();

    private float energy = 100f;
    private float food = 100f;
    private float cleaness = 100f;
    private float happiness = 100f;

    public float Energy
    {
        get
        {
            return energy;
        }
        set
        {
            value = Mathf.Clamp(value, 0f, 100f);
            energy = value;
        }
    }
    public float Food
    {
        get
        {
            return food;
        }
        set
        {
            value = Mathf.Clamp(value, 0f, 100f);
            food = value;
        }
    }
    public float Cleaness
    {
        get
        {
            return cleaness;
        }
        set
        {
            value = Mathf.Clamp(value, 0f, 100f);
            cleaness = value;
        }
    }
    public float Happiness
    {
        get
        {
            return happiness;
        }
        set
        {
            value = Mathf.Clamp(value, 0f, 100f);
            happiness = value;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
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

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeState(beingPickUpState);
            }
        }

        currentState.OnTriggerStay(this, other);
    }
}
