using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingAnimalHandler : PlayerDetect
{
    [SerializeField] private float flyingSpeed = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool followNormal;

    [Header("Parabolic Flight")]
    [SerializeField] private float arcHeight = 2f;   // càng lớn → bay càng vồng

    [SerializeField] private LandableType landableType;

    private Animator animator;
    private Rigidbody rb;

    private FlyingAnimalBaseState currentState;

    #region Properties
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;

    public FlyingAnimalBaseState CurrentState => currentState;
    public FlyingAnimalIdleState idleState = new FlyingAnimalIdleState();
    public FlyingAroundState flyingAroundState = new FlyingAroundState();

    public float FlyingSpeed => flyingSpeed;
    public float RotationSpeed => rotationSpeed;
    public bool FollowNormal => followNormal;
    public float ArcHeight => arcHeight;
    public LandableType LandableType => landableType;
    #endregion

    #region Methods
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentState = idleState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(FlyingAnimalBaseState newState)
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
        currentState.OnInteract(this);
    }

    #endregion
}
