using UnityEngine;

/// <summary>
/// Central system that orchestrates the global saturation value.
/// Combines emotional state + progression phase + restoration bonuses into a single 0-1 saturation value.
/// This value is used by ColorGradingController to desaturate the world and UI.
/// </summary>
public class GlobalSaturationManager : MonoBehaviour
{
    [Header("Game Phases")]
    [SerializeField] private AnimationCurve phaseProgression = AnimationCurve.Linear(0, 1, 1, 0.2f);
    [SerializeField] private float phaseDuration = 1800f; // 30 minutes in seconds

    [Header("Emotional State Influence")]
    [SerializeField] private float emotionalHealthWeight = 0.5f; // How much emotional health affects saturation
    [SerializeField] private float phaseWeight = 0.5f; // How much game phase affects saturation

    [Header("Smoothing")]
    [SerializeField] private float saturationSmoothSpeed = 0.1f; // How quickly saturation transitions

    private float elapsedGameTime = 0f;
    private float currentSaturation = 1f;
    private float targetSaturation = 1f;

    public static GlobalSaturationManager Instance { get; private set; }

    public float CurrentSaturation => currentSaturation;
    public float TargetSaturation => targetSaturation;
    public float ElapsedGameTime => elapsedGameTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        elapsedGameTime += Time.deltaTime;
        UpdateSaturation();
    }

    private void UpdateSaturation()
    {
        // Calculate phase multiplier (1.0 early game → 0.2 late game)
        float phaseProgress = Mathf.Clamp01(elapsedGameTime / phaseDuration);
        float phaseMultiplier = phaseProgression.Evaluate(phaseProgress);

        // Get emotional health from tracker
        float emotionalHealth = EmotionalStateTracker.Instance != null
            ? EmotionalStateTracker.Instance.GetEmotionalHealth()
            : 1f;

        // Base desaturation combines phase + emotional state
        float baseDesaturation = Mathf.Lerp(phaseMultiplier, emotionalHealth, emotionalHealthWeight);

        // Add restoration bonuses
        float restorationBonus = RestorationTracker.Instance != null
            ? RestorationTracker.Instance.GetTotalRestorationBonus()
            : 0f;

        // Final saturation is base + restorations, clamped to 0-1
        targetSaturation = Mathf.Clamp01(baseDesaturation + restorationBonus);

        // Smoothly interpolate current saturation toward target
        currentSaturation = Mathf.Lerp(currentSaturation, targetSaturation, saturationSmoothSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Instantly set the game phase progress (0-1). Useful for testing or cutscenes.
    /// </summary>
    public void SetPhaseProgress(float progress)
    {
        elapsedGameTime = Mathf.Clamp01(progress) * phaseDuration;
    }

    /// <summary>
    /// Reset saturation to full color (useful for testing or special events).
    /// </summary>
    public void ResetSaturation()
    {
        currentSaturation = 1f;
        targetSaturation = 1f;
    }

    private void OnGUI()
    {
        // Temporary debug display
        GUI.Label(new Rect(10, 10, 300, 20), $"Phase Progress: {(elapsedGameTime / phaseDuration):P}");
        GUI.Label(new Rect(10, 35, 300, 20), $"Current Saturation: {currentSaturation:F2}");
    }
}
