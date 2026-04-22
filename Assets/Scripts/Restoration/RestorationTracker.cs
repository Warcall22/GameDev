using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks active restoration bonuses and their remaining durations.
/// Outputs a total restoration bonus value to GlobalSaturationManager.
/// </summary>
public class RestorationTracker : MonoBehaviour
{
    private struct ActiveRestoration
    {
        public RestorationSource source;
        public float timeRemaining;
        public bool hasBeenUsed;
        public float lastUsedTime;
    }

    [SerializeField] private List<RestorationSource> availableRestorations = new List<RestorationSource>();

    private List<ActiveRestoration> activeRestorations = new List<ActiveRestoration>();
    private Dictionary<RestorationSource, int> sourceIndexMap = new Dictionary<RestorationSource, int>();

    public static RestorationTracker Instance { get; private set; }

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

        // Initialize restoration tracking
        for (int i = 0; i < availableRestorations.Count; i++)
        {
            if (availableRestorations[i] != null)
            {
                sourceIndexMap[availableRestorations[i]] = i;
            }
        }
    }

    private void Update()
    {
        // Decay active restoration bonuses
        for (int i = activeRestorations.Count - 1; i >= 0; i--)
        {
            ActiveRestoration restoration = activeRestorations[i];
            restoration.timeRemaining -= Time.deltaTime;

            if (restoration.timeRemaining <= 0)
            {
                activeRestorations.RemoveAt(i);
            }
            else
            {
                activeRestorations[i] = restoration;
            }
        }
    }

    /// <summary>
    /// Apply a restoration from the given source.
    /// Checks if the emotional health requirement is met and usage rules are satisfied.
    /// Returns true if restoration was successfully applied.
    /// </summary>
    public bool ApplyRestoration(RestorationSource source)
    {
        if (source == null)
        {
            Debug.LogWarning("RestorationTracker: Attempted to apply null restoration source");
            return false;
        }

        // Check emotional health requirement
        float emotionalHealth = EmotionalStateTracker.Instance != null
            ? EmotionalStateTracker.Instance.GetEmotionalHealth()
            : 0.5f;

        if (emotionalHealth < source.MinEmotionalHealthRequirement)
        {
            Debug.Log($"Restoration '{source.SourceName}' blocked: emotional health {emotionalHealth:F2} below requirement {source.MinEmotionalHealthRequirement:F2}");
            return false;
        }

        // Find if this restoration is already active
        int existingIndex = activeRestorations.FindIndex(r => r.source == source);

        if (existingIndex >= 0)
        {
            // Restoration already active
            ActiveRestoration existing = activeRestorations[existingIndex];

            if (!source.CanUseMultipleTimes)
            {
                // Can't apply again, just refresh duration
                existing.timeRemaining = source.BonusDuration;
                activeRestorations[existingIndex] = existing;
                return true;
            }

            // Check cooldown for multi-use restorations
            if (Time.time - existing.lastUsedTime < source.CooldownBetweenUses)
            {
                Debug.Log($"Restoration '{source.SourceName}' is on cooldown");
                return false;
            }

            // Refresh and update last used time
            existing.timeRemaining = source.BonusDuration;
            existing.lastUsedTime = Time.time;
            activeRestorations[existingIndex] = existing;
        }
        else
        {
            // Add new active restoration
            ActiveRestoration newRestoration = new ActiveRestoration
            {
                source = source,
                timeRemaining = source.BonusDuration,
                hasBeenUsed = true,
                lastUsedTime = Time.time
            };
            activeRestorations.Add(newRestoration);
        }

        // Play feedback
        PlayRestorationFeedback(source);

        Debug.Log($"Restoration '{source.SourceName}' applied! Bonus: +{source.SaturationBonus:F2} saturation for {source.BonusDuration}s");
        return true;
    }

    /// <summary>
    /// Get the total restoration bonus to add to base saturation (0-1).
    /// </summary>
    public float GetTotalRestorationBonus()
    {
        float totalBonus = 0f;

        foreach (var restoration in activeRestorations)
        {
            // Weight bonus by remaining time (so it fades out smoothly)
            float bonusWeight = restoration.timeRemaining / restoration.source.BonusDuration;
            totalBonus += restoration.source.SaturationBonus * bonusWeight;
        }

        return Mathf.Clamp01(totalBonus);
    }

    /// <summary>
    /// Clear all active restorations (useful for testing or new game).
    /// </summary>
    public void ClearAllRestorations()
    {
        activeRestorations.Clear();
    }

    private void PlayRestorationFeedback(RestorationSource source)
    {
        // Visual feedback: flash screen or object glow
        // Audio feedback: play sound if available
        if (source.FeedbackSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(source.FeedbackSound);
            }
        }
    }

    private void OnGUI()
    {
        // Debug display of active restorations
        GUI.Label(new Rect(10, 160, 300, 20), $"Active Restorations: {activeRestorations.Count}");
        for (int i = 0; i < activeRestorations.Count; i++)
        {
            GUI.Label(new Rect(10, 185 + i * 25, 300, 20),
                $"  {activeRestorations[i].source.SourceName}: {activeRestorations[i].timeRemaining:F1}s remaining");
        }
        GUI.Label(new Rect(10, 160 + (activeRestorations.Count + 1) * 25, 300, 20),
            $"Total Bonus: +{GetTotalRestorationBonus():F2}");
    }
}
