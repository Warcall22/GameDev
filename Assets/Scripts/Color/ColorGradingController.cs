using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Applies global saturation desaturation via post-processing color grading.
/// Adjusts contrast to keep UI readable as saturation fades.
/// </summary>
public class ColorGradingController : MonoBehaviour
{
    [Header("Post-Processing")]
    [SerializeField] private PostProcessVolume postProcessVolume;

    [Header("UI Readability")]
    [SerializeField] private float minContrast = 1.2f; // Min contrast when saturation = 0 (grayscale)
    [SerializeField] private float maxContrast = 1.0f; // Max contrast when saturation = 1 (full color)

    private ColorGrading colorGrading;

    private void Start()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = GetComponent<PostProcessVolume>();
        }

        if (postProcessVolume == null)
        {
            Debug.LogError("ColorGradingController: No PostProcessVolume found! Add one to this GameObject or assign it manually.");
            return;
        }

        // Get or create the ColorGrading effect
        if (!postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
            postProcessVolume.profile.AddSettings(colorGrading);
        }

        // Initialize color grading settings
        colorGrading.saturation.overrideState = true;
        colorGrading.contrast.overrideState = true;
    }

    private void Update()
    {
        if (colorGrading == null || GlobalSaturationManager.Instance == null)
            return;

        float saturation = GlobalSaturationManager.Instance.CurrentSaturation;

        // Saturation goes from -100 (grayscale) to 0 (normal) to 100 (oversaturated)
        // We want full saturation at 1.0 and -100 (grayscale) at 0.0
        colorGrading.saturation.value = Mathf.Lerp(-100f, 0f, saturation);

        // Increase contrast as saturation decreases to keep UI readable
        float contrast = Mathf.Lerp(minContrast, maxContrast, saturation);
        colorGrading.contrast.value = Mathf.Lerp(0f, 20f, 1f - contrast); // Contrast in -100 to 100 range
    }
}
