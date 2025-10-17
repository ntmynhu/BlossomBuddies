using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetStateManager : MonoBehaviour
{
    [SerializeField] private List<PetStateRateEntry> petStateRates;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Rigidbody rb;

    private PetBaseState currentState;

    private float energy = 100f;
    private float food = 100f;
    private float cleaness = 100f;
    private float happiness = 100f;

    private Dictionary<PetStateType, PetStatsRate> petRateDict;

    #region Properties
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
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
    public float Cleanliness
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

    public WalkAroundState walkAroundState = new WalkAroundState();
    public SleepingState sleepingState = new SleepingState();
    public EatingState eatingState = new EatingState();
    public ChasingPlayer chasingPlayerState = new ChasingPlayer();
    public RunAwayFromPlayer runAwayFromPlayerState = new RunAwayFromPlayer();
    public BeingPickUp beingPickUpState = new BeingPickUp();

    public Dictionary<PetStateType, PetStatsRate> PetRateDict => petRateDict;
    #endregion

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        petRateDict = new Dictionary<PetStateType, PetStatsRate>();
        foreach (var entry in petStateRates)
        {
            petRateDict[entry.state] = entry.rate;
        }
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

    public void ChangeState(PetBaseState newState)
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

[System.Serializable]
public class PetStateRateEntry
{
    public PetStateType state;
    public PetStatsRate rate;
}

public enum PetStateType
{
    Sleep,
    Eat,
    Bath,
    WalkAround,
    Play,
    FindPlayer,
    AvoidPlayer,
    BeingPickup
}
