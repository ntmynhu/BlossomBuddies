using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LandableAutoRegister : MonoBehaviour
{
    [Tooltip("If true, this landable will be ignored by raycasts. Useful when the land is inside another collider")]
    [SerializeField] private bool ignoreRayCast = false;
    [SerializeField] private LandableType landableType;
    private Collider col;

    public bool IsIgnoreRayCast => ignoreRayCast;
    public LandableType LandableType => landableType;
    public Collider Collider => col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        if (LandableRegistry.Instance != null)
            LandableRegistry.Instance.Register(this);
    }

    private void OnEnable()
    {
        if (LandableRegistry.Instance != null)
            LandableRegistry.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (LandableRegistry.Instance != null)
            LandableRegistry.Instance.Unregister(this);
    }
}
