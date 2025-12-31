using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LandableAutoRegister : MonoBehaviour
{
    [SerializeField] private bool ignoreRayCast = false;
    private Collider col;

    public bool IsIgnoreRayCast => ignoreRayCast;
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
