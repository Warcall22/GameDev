using UnityEngine;

/// <summary>
/// Tracks the player's emotional responses to grief based on behavior.
/// Measures three emotional states: Avoidance, Clinging, and Acceptance.
/// Outputs a composite "emotional health" value (0-1) that influences color desaturation.
/// </summary>
public class EmotionalStateTracker : MonoBehaviour
{
    [Header("Emotional State Scores")]
    [Range(0, 100)] public float avoidanceScore = 0f;
    [Range(0, 100)] public float clingingScore = 0f;
    [Range(0, 100)] public float acceptanceScore = 0f;

    [Header("Score Decay")]
    [SerializeField] private float decayRate = 0.1f; // How quickly scores decay over time

    [Header("Behavioral Thresholds")]
    [SerializeField] private float quickLeaveThreshold = 2f; // seconds before counting as avoidance
    [SerializeField] private float prolongedInteractionThreshold = 10f; // seconds before counting as clinging

    private float currentMemoryInteractionTime = 0f;
    private bool isInteractingWithMemory = false;

    public static EmotionalStateTracker Instance { get; private set; }

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
        DecayScores();
    }

    /// <summary>
    /// Get the composite emotional health value (0-1).
    /// 1.0 = perfect acceptance, 0.0 = avoidance or clinging.
    /// </summary>
    public float GetEmotionalHealth()
    {
        float totalScore = avoidanceScore + clingingScore + acceptanceScore;
        
        if (totalScore == 0) return 1f; // Default to healthy if no data
        
        // Acceptance contributes positively, avoidance/clinging reduce health
        float health = acceptanceScore / totalScore;
        return Mathf.Clamp01(health);
    }

    /// <summary>
    /// Called when player starts interacting with a memory.
    /// </summary>
    public void BeginMemoryInteraction()
    {
        isInteractingWithMemory = true;
        currentMemoryInteractionTime = 0f;
    }

    /// <summary>
    /// Called when player finishes interacting with a memory.
    /// Duration determines if it counts as avoidance, acceptance, or clinging.
    /// </summary>
    public void EndMemoryInteraction()
    {
        if (!isInteractingWithMemory) return;

        isInteractingWithMemory = false;

        if (currentMemoryInteractionTime < quickLeaveThreshold)
        {
            // Quick exit = avoidance
            AddAvoidanceScore(10f);
        }
        else if (currentMemoryInteractionTime > prolongedInteractionThreshold)
        {
            // Prolonged interaction = clinging
            AddClingingScore(5f);
        }
        else
        {
            // Balanced interaction = acceptance
            AddAcceptanceScore(15f);
        }

        currentMemoryInteractionTime = 0f;
    }

    /// <summary>
    /// Called when player moves through an area quickly (sprinting/rushing).
    /// </summary>
    public void RecordFastMovement()
    {
        AddAvoidanceScore(3f);
    }

    /// <summary>
    /// Called when player explores deliberately and moves forward.
    /// </summary>
    public void RecordBalancedExploration()
    {
        AddAcceptanceScore(5f);
    }

    /// <summary>
    /// Called when player lingers in an area without progressing.
    /// </summary>
    public void RecordProlongedLingers()
    {
        AddClingingScore(4f);
    }

    /// <summary>
    /// Called when player uses color restoration.
    /// </summary>
    public void RecordRestorationUsage(bool isOveruse = false)
    {
        if (isOveruse)
        {
            AddClingingScore(8f); // Overuse of restoration = denial/clinging
        }
        else
        {
            AddAcceptanceScore(10f); // Healthy restoration = acceptance
        }
    }

    private void AddAvoidanceScore(float amount)
    {
        avoidanceScore = Mathf.Min(avoidanceScore + amount, 100f);
    }

    private void AddClingingScore(float amount)
    {
        clingingScore = Mathf.Min(clingingScore + amount, 100f);
    }

    private void AddAcceptanceScore(float amount)
    {
        acceptanceScore = Mathf.Min(acceptanceScore + amount, 100f);
    }

    private void DecayScores()
    {
        // All scores decay slightly over time to reset the player's emotional state gradually
        float deltaDecay = decayRate * Time.deltaTime;
        avoidanceScore = Mathf.Max(avoidanceScore - deltaDecay, 0f);
        clingingScore = Mathf.Max(clingingScore - deltaDecay, 0f);
        acceptanceScore = Mathf.Max(acceptanceScore - deltaDecay, 0f);
    }

    private void OnGUI()
    {
        // Temporary debug display
        GUI.Label(new Rect(10, 60, 300, 20), $"Avoidance: {avoidanceScore:F1}");
        GUI.Label(new Rect(10, 85, 300, 20), $"Clinging: {clingingScore:F1}");
        GUI.Label(new Rect(10, 110, 300, 20), $"Acceptance: {acceptanceScore:F1}");
        GUI.Label(new Rect(10, 135, 300, 20), $"Emotional Health: {GetEmotionalHealth():F2}");
    }
}
