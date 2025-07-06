using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class SingularityController : MonoBehaviour
{
    public static event Action OnSingularityClicked;

    [Header("Level Progression")]
    public List<LevelData> gameLevels;

    [Header("Component References")]
    public Volume postProcessingVolume;
    public ObjectSpawner objectSpawner;
    public AudioSource sfxAudioSource;
    public SpriteRenderer corePatternRenderer;
    public SpriteRenderer coreBGRenderer;
    public AudioSource musicAudioSource;
    public bool enableEnding = true;
    // --- Internal State & Component References ---
    private int currentLevelIndex = 0;
    private int absorbedInCurrentLevel = 0;
    private LevelData currentLevel;
    private SpriteRenderer buttonSpriteRenderer; // Reference to our own sprite renderer
    private Camera mainCamera;
    private LensDistortion lensDistortion;
    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ScreenSpaceLensFlare screenSpaceLensFlares;
    private Coroutine distortionCoroutine;
    private Coroutine cameraZoomCoroutine;
    private bool isEnding = false; // Prevents multiple clicks during the ending


    void Start()
    {
        buttonSpriteRenderer = GetComponent<SpriteRenderer>(); 
        mainCamera = Camera.main;
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out lensDistortion);
            postProcessingVolume.profile.TryGet(out bloom);
            postProcessingVolume.profile.TryGet(out vignette);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out screenSpaceLensFlares);
        }
        if (gameLevels == null || gameLevels.Count == 0) { this.enabled = false; return; }
        TransitionToLevel(0);
    }

    void OnButtonClicked()
    {
        ClickData clickData = currentLevel.clickParameters;
        if (clickData == null) return;

        // The old core reveal logic that was here is now removed.

        OnSingularityClicked?.Invoke();
        if (CameraShaker.Instance != null) CameraShaker.Instance.Shake(clickData.cameraShakeIntensity);
        if (distortionCoroutine != null) StopCoroutine(distortionCoroutine);
        distortionCoroutine = StartCoroutine(DistortionPulse(clickData.distortionPeakIntensity, clickData.distortionDuration));

        if (sfxAudioSource != null && clickData.clickSounds != null && clickData.clickSounds.Count > 0)
        {
            sfxAudioSource.clip = clickData.clickSounds[UnityEngine.Random.Range(0, clickData.clickSounds.Count)];
            sfxAudioSource.Play();
        }

        if (clickData.shockwavePrefab != null)
        {
            GameObject shockwaveInstance = Instantiate(clickData.shockwavePrefab, transform.position, Quaternion.identity);
            if (shockwaveInstance.TryGetComponent(out ProceduralShockwave shockwave))
            {
                shockwave.Initialize(clickData.shockwaveDuration, clickData.shockwaveEndScale, clickData.shockwaveColor, clickData.shockwaveRotationSpeed);
            }
        }
    }
    public void PlayAbsorptionSound()
    {
        if (currentLevel.absorptionSounds != null && currentLevel.absorptionSounds.Count > 0)
        {
            AudioClip randomSound = currentLevel.absorptionSounds[Random.Range(0, currentLevel.absorptionSounds.Count)];
            sfxAudioSource.PlayOneShot(randomSound);
        }
    }

    void TransitionToLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        currentLevel = gameLevels[currentLevelIndex];
        absorbedInCurrentLevel = 0;

        if (currentLevel != null && currentLevel.transitionSounds != null && currentLevel.transitionSounds.Count > 0)
        {
            AudioClip randomSound = currentLevel.transitionSounds[Random.Range(0, currentLevel.transitionSounds.Count)];
            sfxAudioSource.PlayOneShot(randomSound);
        }
        if (objectSpawner != null) objectSpawner.ConfigureForLevel(currentLevel);
        if (cameraZoomCoroutine != null) StopCoroutine(cameraZoomCoroutine);
        cameraZoomCoroutine = StartCoroutine(AnimateCameraZoom(currentLevel.cameraZoom, 1.0f));
        if (corePatternRenderer != null)
        {
            corePatternRenderer.color = currentLevel.corePatternColor;
        }
        if (bloom != null) bloom.intensity.value = currentLevel.bloomIntensity;
        if (vignette != null) vignette.intensity.value = currentLevel.vignetteIntensity;
        if (chromaticAberration != null) chromaticAberration.intensity.value = currentLevel.chromaticAberrationIntensity;
        if (screenSpaceLensFlares != null) screenSpaceLensFlares.intensity.value = currentLevel.screenSpaceLensFlareIntensity;
        if (coreBGRenderer!=null) coreBGRenderer.color = currentLevel.coreBGColor;
        if (buttonSpriteRenderer != null)
        {
            buttonSpriteRenderer.color = currentLevel.levelColor;
        }

        if (musicAudioSource != null)
        {
            // Set the volume for the new track
            musicAudioSource.volume = currentLevel.musicVolume;

            if (currentLevel.soundtrack != null && musicAudioSource.clip != currentLevel.soundtrack)
            {
                musicAudioSource.clip = currentLevel.soundtrack;
                musicAudioSource.Play();
            }
        }

        Debug.Log("Transitioned to Level: " + currentLevel.name);
    }


    #region Unchanged Methods
    public void NotifyOfAbsorption()
    {
        absorbedInCurrentLevel++;
        transform.localScale += Vector3.one * currentLevel.growthPerAbsorption;
        CheckForLevelUp();
    }

    void CheckForLevelUp()
    {
        // If we have enough absorptions...
        if (absorbedInCurrentLevel >= currentLevel.objectsToAbsorbForNextLevel)
        {
            // And if there is a next level to go to...
            if (currentLevelIndex + 1 < gameLevels.Count)
            {
                TransitionToLevel(currentLevelIndex + 1);
            }
            // Otherwise, if this was the final level...
            else if (!isEnding && enableEnding)
            {
                // Start the ending sequence!
                StartCoroutine(EndingSequence());
            }
        }
    }

    IEnumerator EndingSequence()
    {
        isEnding = true;
        float duration = 3.0f; // How long the ending animation takes
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        // End scale should be large enough to cover the screen
        Vector3 endScale = Vector3.one * 50f;

        // Optional: Fade out all orbiting objects
        // FindObjectsByType<PhysicsOrbitalMovement>(FindObjectsSortMode.None) ... and fade them out.

        while (elapsedTime < duration)
        {
            // Grow the singularity exponentially to cover the screen
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);

            // Zoom the camera in at the same time
            mainCamera.orthographicSize = Mathf.Lerp(currentLevel.cameraZoom, 0.1f, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Restart the game by reloading the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If we are in the Unity Editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            // If we are in a built game
#else
                Application.Quit();
#endif
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                OnButtonClicked();
            }
        }
    }

    IEnumerator AnimateCameraZoom(float targetSize, float duration)
    {
        float elapsedTime = 0;
        float startingSize = mainCamera.orthographicSize;
        while (elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startingSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.orthographicSize = targetSize;
    }

    IEnumerator DistortionPulse(float peakIntensity, float duration)
    {
        if (lensDistortion == null) yield break;
        float elapsedTime = 0f;
        float halfDuration = duration / 2f;
        while (elapsedTime < halfDuration)
        {
            lensDistortion.intensity.value = Mathf.Lerp(0, peakIntensity, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            lensDistortion.intensity.value = Mathf.Lerp(peakIntensity, 0, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lensDistortion.intensity.value = 0f;
    }
    #endregion
}