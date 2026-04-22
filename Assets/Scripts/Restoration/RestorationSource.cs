using UnityEngine;

/// <summary>
/// Defines a source of color restoration (memory, item, emotional milestone).
/// Can be used once or multiple times depending on configuration.
/// Restoration bonus only applies if emotional health is above the threshold.
/// </summary>
[CreateAssetMenu(fileName = "New Restoration Source", menuName = "PRIMUS/Restoration Source")]
public class RestorationSource : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string sourceName = "Memory";
    [SerializeField] private string sourceDescription = "A moment of warmth and connection";

    [Header("Restoration Effect")]
    [SerializeField] private float saturationBonus = 0.1f; // 0-1, how much saturation to restore
    [SerializeField] private float bonusDuration = 30f; // How long the bonus lasts (seconds)

    [Header("Usage Rules")]
    [SerializeField] private bool canUseMultipleTimes = false; // If false, bonus only applies once
    [SerializeField] private float minEmotionalHealthRequirement = 0.2f; // Must be above this to gain bonus
    [SerializeField] private float cooldownBetweenUses = 60f; // Seconds before can use again (if multi-use)

    [Header("Visual/Audio Feedback")]
    [SerializeField] private Color feedbackColor = Color.white;
    [SerializeField] private AudioClip feedbackSound;

    public string SourceName => sourceName;
    public string SourceDescription => sourceDescription;
    public float SaturationBonus => Mathf.Clamp01(saturationBonus);
    public float BonusDuration => bonusDuration;
    public bool CanUseMultipleTimes => canUseMultipleTimes;
    public float MinEmotionalHealthRequirement => Mathf.Clamp01(minEmotionalHealthRequirement);
    public float CooldownBetweenUses => cooldownBetweenUses;
    public Color FeedbackColor => feedbackColor;
    public AudioClip FeedbackSound => feedbackSound;
}
