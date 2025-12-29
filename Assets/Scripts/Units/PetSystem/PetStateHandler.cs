using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetStateHandler : PlayerDetect
{
    #region Fields
    [SerializeField] private List<PetStateRate> petStateRates;
    [SerializeField] private GameObject[] bubbles;
    [SerializeField] private Renderer petRenderer;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Rigidbody rb;

    private PetBaseState currentState;

    private float energy = Global.MAX_STAT_VALUE;
    private float food = Global.MAX_STAT_VALUE;
    private float cleaness = Global.MAX_STAT_VALUE;
    private float happiness = Global.MAX_STAT_VALUE;

    private Dictionary<PetStateType, PetStatsRate> petRateDict;
    #endregion

    #region Properties
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    public PetBaseState CurrentState => currentState;
    public float Energy
    {
        get
        {
            return energy;
        }
        set
        {
            value = Mathf.Clamp(value, 0f, Global.MAX_STAT_VALUE);
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
            value = Mathf.Clamp(value, 0f, Global.MAX_STAT_VALUE);
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
            value = Mathf.Clamp(value, 0f, Global.MAX_STAT_VALUE);
            cleaness = value;
            OnCleanlinessChanged();
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
            value = Mathf.Clamp(value, 0f, Global.MAX_STAT_VALUE);
            happiness = value;
        }
    }

    public WalkAroundState walkAroundState = new WalkAroundState();
    public SleepingState sleepingState = new SleepingState();
    public EatingState eatingState = new EatingState();
    public ChasingPlayer chasingPlayerState = new ChasingPlayer();
    public RunAwayFromPlayer runAwayFromPlayerState = new RunAwayFromPlayer();
    public BeingPickUp beingPickUpState = new BeingPickUp();
    public BathingState bathingState = new BathingState();

    public Dictionary<PetStateType, PetStatsRate> PetRateDict => petRateDict;
    public GameObject[] Bubbles => bubbles;
    #endregion

    #region Methods
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

    private void OnCleanlinessChanged()
    {
        petRenderer.materials[0].SetFloat("_Dirt", 1f - (cleaness / Global.MAX_STAT_VALUE));
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        currentState.OnTriggerEnter(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(this, other);
    }

    private void OnInteract()
    {
        if (isPlayerInRange && currentState != bathingState)
        {
            ChangeState(beingPickUpState);
        }

        currentState.OnInteract(this);
    }

    #endregion
}

[System.Serializable]
public class PetStateRate
{
    public PetStateType state;
    public PetStatsRate rate;
}

public enum PetStateType
{
    Sleep,
    Eat,
    Bathing,
    WalkAround,
    Play,
    FindPlayer,
    AvoidPlayer,
    BeingPickup,
}
