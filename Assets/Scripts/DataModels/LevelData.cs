using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level_01_Data", menuName = "Singularity/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level State & Progression")]
    public int objectsToAbsorbForNextLevel;
    public float cameraZoom = 5f;
    public float growthPerAbsorption = 0.05f;

    [Header("Level Appearance")]
    public Color levelColor = Color.white; // The main color for the singularity in this level
    public Color corePatternColor = Color.cyan;
    public Color coreBGColor = Color.white;
    [Header("Level Audio")]
    public AudioClip soundtrack;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    public List<AudioClip> absorptionSounds; 
    public List<AudioClip> transitionSounds; 

    [Header("Object Spawning")]
    public List<GameObject> absorbablePrefabs;
    public float generationRate = 1f;
    public int maxObjects = 40; 

    [Header("Post-Processing Intensity")]
    public float bloomIntensity = 1f;
    public float vignetteIntensity = 0.2f;
    public float chromaticAberrationIntensity = 0f;
    public float screenSpaceLensFlareIntensity = 0f;

    [Header("Click Parameters")]
    public ClickData clickParameters;
}