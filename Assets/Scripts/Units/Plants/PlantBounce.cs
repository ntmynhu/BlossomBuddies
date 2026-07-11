using UnityEngine;

/// <summary>
/// Drives the shared plant "bounce" animation from the plant's growth state:
/// - a strong, springy bounce while thriving (conditions met, growing);
/// - a gentle, drooping bounce while declining (lacking conditions).
/// Uses an Animator + the editable "PlantFeedback" controller (Resources/Animations).
/// Attached to the same GameObject as <see cref="Plant"/> so it animates the plant root scale.
/// </summary>
[RequireComponent(typeof(Plant))]
public class PlantBounce : MonoBehaviour
{
    private const string ControllerPath = "Animations/PlantFeedback";
    private const string IsGrowingParam = "IsGrowing";

    private Plant _plant;
    private Animator _animator;
    private Vector3 _baseScale;

    public static void Attach(Plant plant)
    {
        if (plant.GetComponent<PlantBounce>() != null) return;
        plant.gameObject.AddComponent<PlantBounce>();
    }

    private void Awake()
    {
        _plant = GetComponent<Plant>();
        _baseScale = transform.localScale;

        _animator = GetComponent<Animator>();
        if (_animator == null) _animator = gameObject.AddComponent<Animator>();

        var controller = Resources.Load<RuntimeAnimatorController>(ControllerPath);
        if (controller != null) _animator.runtimeAnimatorController = controller;

        _animator.enabled = false; // enabled only while the plant is actively growing
    }

    private void LateUpdate()
    {
        if (_plant == null || _animator == null || _animator.runtimeAnimatorController == null)
            return;

        bool active = !_plant.IsDead && !_plant.IsFullyGrown && _plant.AreStatsReady;

        if (active)
        {
            if (!_animator.enabled) _animator.enabled = true;
            _animator.SetBool(IsGrowingParam, _plant.IsGrowing);
        }
        else if (_animator.enabled)
        {
            _animator.enabled = false;
            transform.localScale = _baseScale; // stop mid-bounce cleanly
        }
    }
}
