using UnityEngine;

/// <summary>
/// Tag component that marks an object or material to retain its color
/// and not be desaturated by the global color fade mechanic.
/// 
/// Use this for:
/// - Key emotional memories/landmarks
/// - Warm light sources that should stay visible
/// - Objects that break the visual monotony and provide hope
/// </summary>
[DisallowMultipleComponent]
public class RetainColorComponent : MonoBehaviour
{
    [Header("Color Retention")]
    [SerializeField] private bool retainColor = true;
    [SerializeField] private float colorRetentionStrength = 1f; // 0-1, how much color to retain relative to global saturation

    [Header("Visual Feedback")]
    [SerializeField] private bool highlightInEditor = true;
    [SerializeField] private Color editorHighlightColor = new Color(1f, 0.84f, 0f, 0.3f); // Gold highlight

    public bool ShouldRetainColor => retainColor;
    public float ColorRetentionStrength => Mathf.Clamp01(colorRetentionStrength);

    private void OnDrawGizmosSelected()
    {
        if (!highlightInEditor) return;

        // Draw a wire sphere around the object to indicate it retains color
        Gizmos.color = editorHighlightColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!highlightInEditor || !retainColor) return;

        Gizmos.color = new Color(1f, 0.84f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
#endif
}
