using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level_01_ClickData", menuName = "Singularity/Click Data")]
public class ClickData : ScriptableObject
{
    [Header("Click Feedback")]
    public List<AudioClip> clickSounds;
    public GameObject shockwavePrefab;

    [Header("Shockwave Parameters")]
    public float shockwaveDuration = 0.5f;
    public float shockwaveEndScale = 8f;
    public Color shockwaveColor = Color.white;
    public float shockwaveRotationSpeed = 5f; // Degrees per second


    [Header("Juice Parameters")]
    public float cameraShakeIntensity;
    public float distortionPeakIntensity;
    public float distortionDuration = 0.5f;
}