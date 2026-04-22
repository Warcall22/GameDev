using UnityEngine;

/// <summary>
/// Demo/Test script for the color fade mechanic system.
/// Attach this to a game object in your test scene and it will set up the color fade system.
/// 
/// Key components:
/// - GlobalSaturationManager: Orchestrates saturation value
/// - EmotionalStateTracker: Tracks player's emotional state
/// - ColorGradingController: Applies post-processing desaturation
/// - RestorationTracker: Manages color restoration bonuses
/// </summary>
public class ColorFade : MonoBehaviour
{
    [Header("System Setup")]
    [SerializeField] private bool autoCreateManagers = true;

    [Header("Test Controls")]
    [SerializeField] private bool showTestUI = true;
    [SerializeField] private float testMemoryInteractionDuration = 5f;

    private void Start()
    {
        if (autoCreateManagers)
        {
            SetupColorFadeSystem();
        }
    }

    private void Update()
    {
        // Test controls
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestMemoryInteraction();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TestAvoidanceBehavior();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TestClingingBehavior();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestAcceptanceBehavior();
        }
    }

    /// <summary>
    /// Sets up all required managers for the color fade system.
    /// Call this once at game start or manually if autoCreateManagers is false.
    /// </summary>
    public void SetupColorFadeSystem()
    {
        // Create EmotionalStateTracker
        if (EmotionalStateTracker.Instance == null)
        {
            GameObject trackerObj = new GameObject("EmotionalStateTracker");
            trackerObj.AddComponent<EmotionalStateTracker>();
            Debug.Log("Created EmotionalStateTracker");
        }

        // Create GlobalSaturationManager
        if (GlobalSaturationManager.Instance == null)
        {
            GameObject saturationObj = new GameObject("GlobalSaturationManager");
            saturationObj.AddComponent<GlobalSaturationManager>();
            Debug.Log("Created GlobalSaturationManager");
        }

        // Create RestorationTracker
        if (RestorationTracker.Instance == null)
        {
            GameObject restorationObj = new GameObject("RestorationTracker");
            restorationObj.AddComponent<AudioSource>();
            restorationObj.AddComponent<RestorationTracker>();
            Debug.Log("Created RestorationTracker");
        }

        // Add ColorGradingController to the main camera
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.GetComponent<ColorGradingController>() == null)
        {
            mainCam.gameObject.AddComponent<ColorGradingController>();
            Debug.Log("Added ColorGradingController to Main Camera");
        }

        Debug.Log("Color Fade System initialized!");
    }

    /// <summary>
    /// Test memory interaction (balanced engagement = acceptance).
    /// </summary>
    private void TestMemoryInteraction()
    {
        if (EmotionalStateTracker.Instance == null) return;

        EmotionalStateTracker.Instance.BeginMemoryInteraction();
        Invoke(nameof(EndTestMemoryInteraction), testMemoryInteractionDuration);
        Debug.Log("Memory interaction started (will end in " + testMemoryInteractionDuration + "s)");
    }

    private void EndTestMemoryInteraction()
    {
        if (EmotionalStateTracker.Instance != null)
        {
            EmotionalStateTracker.Instance.EndMemoryInteraction();
            Debug.Log("Memory interaction ended - Acceptance recorded");
        }
    }

    /// <summary>
    /// Test avoidance behavior (quick exit, sprinting).
    /// </summary>
    private void TestAvoidanceBehavior()
    {
        if (EmotionalStateTracker.Instance == null) return;

        EmotionalStateTracker.Instance.RecordFastMovement();
        Debug.Log("Avoidance behavior recorded");
    }

    /// <summary>
    /// Test clinging behavior (prolonged lingering).
    /// </summary>
    private void TestClingingBehavior()
    {
        if (EmotionalStateTracker.Instance == null) return;

        EmotionalStateTracker.Instance.RecordProlongedLingers();
        Debug.Log("Clinging behavior recorded");
    }

    /// <summary>
    /// Test acceptance behavior (balanced exploration).
    /// </summary>
    private void TestAcceptanceBehavior()
    {
        if (EmotionalStateTracker.Instance == null) return;

        EmotionalStateTracker.Instance.RecordBalancedExploration();
        Debug.Log("Acceptance behavior recorded");
    }

    private void OnGUI()
    {
        if (!showTestUI) return;

        GUILayout.BeginArea(new Rect(320, 10, 400, 300));
        
        GUILayout.Label("COLOR FADE SYSTEM - TEST CONTROLS", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold });
        GUILayout.Space(10);

        GUILayout.Label("Press E: Start memory interaction (acceptance if ~5s)");
        GUILayout.Label("Press R: Record fast movement (avoidance)");
        GUILayout.Label("Press C: Record prolonged lingering (clinging)");
        GUILayout.Label("Press Space: Record balanced exploration (acceptance)");

        GUILayout.Space(10);
        GUILayout.Label("System Status:");
        
        if (GlobalSaturationManager.Instance != null)
        {
            GUILayout.Label($"  ✓ GlobalSaturationManager active");
        }
        else
        {
            GUILayout.Label($"  ✗ GlobalSaturationManager NOT found");
        }

        if (EmotionalStateTracker.Instance != null)
        {
            GUILayout.Label($"  ✓ EmotionalStateTracker active");
        }
        else
        {
            GUILayout.Label($"  ✗ EmotionalStateTracker NOT found");
        }

        if (RestorationTracker.Instance != null)
        {
            GUILayout.Label($"  ✓ RestorationTracker active");
        }
        else
        {
            GUILayout.Label($"  ✗ RestorationTracker NOT found");
        }

        GUILayout.EndArea();
    }
}
