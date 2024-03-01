using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;
using System.Linq;
using System.Reflection;


public enum ESoundType
{
    playerAttack,
    playerAttackCrit,
    playerDeath,
    playerEnterPortal,
    playerAttackHit,
    playerAttackHitCrit,
    playerCollectTry,
    playerCollectResource,
    playerCollectHuman,
    playerDropOff,
    humanShotgun,
    humanAssaultRifle,
    humanGatlingGun,
    humanPitchfork,
    humanDeath,
    buildingOrganHarvester,
    buildingBoneHarvester,
    buildingBloodHarvester,
    buildingImpact,
    buildingDeath,
    buttonClick,
    badSelection,
    menuOpen,
    menuClose,
    constructionComplete,
    homeAmbient,
    farmWildAmbient,
    homeMusic,
    farmMusic,
    playerDodge,
}
public enum ESoundSource
{
    Player,
    Husband,
    Human,
    Shotgun,
    Pitchfork,
    AssaultRifle,
    GatlingGun,
    Building,
    UI,
    Ambient,
    Music,
}
public struct SoundRequest
{
    public ESoundSource SoundSource;
    public GameObject RequestingObject;
    public ESoundType SoundType;
    public bool RandomizePitch, Loop;
}
[System.Serializable]
struct SoundSource
{
    public ESoundSource Source;
    public int instances;
}

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public int audioSourceCountPerType = 6;
    private Dictionary<ESoundSource, List<AudioSource>> audioSourcePools;
    [SerializeField] public Dictionary<string, AudioClip[]> AudioDictionary = new();//
    [SerializeField] List<SoundSource> soundSources = new List<SoundSource>();

    #region  AudioClips
    [Header("Player Clips")]
    [SerializeField] AudioClip[] playerAttack;
    [SerializeField] AudioClip[] playerAttackCrit;
    [SerializeField] AudioClip[] playerDeath;
    [SerializeField] AudioClip[] playerEnterPortal;
    [SerializeField] AudioClip[] playerAttackHit;
    [SerializeField] AudioClip[] playerAttackHitCrit;
    [SerializeField] AudioClip[] playerCollectTry;
    [SerializeField] AudioClip[] playerCollectResource;
    [SerializeField] AudioClip[] playerCollectHuman;
    [SerializeField] AudioClip[] playerDropOff;
    [SerializeField] AudioClip[] playerDodge;

    [Header("Human Clips")]
    [SerializeField] AudioClip[] humanShotgun;
    [SerializeField] AudioClip[] humanAssaultRifle;
    [SerializeField] AudioClip[] humanGatlingGun;
    [SerializeField] AudioClip[] humanPitchfork;
    [SerializeField] AudioClip[] humanDeath;

    [Header("Building Clips")]
    [SerializeField] AudioClip[] buildingOrganHarvester;
    [SerializeField] AudioClip[] buildingBoneHarvester;
    [SerializeField] AudioClip[] buildingBloodHarvester;

    [Header("Destructable Clips")]
    [SerializeField] AudioClip[] buildingImpact;
    [SerializeField] AudioClip[] buildingDeath;

    [Header("UI Clips")]
    [SerializeField] AudioClip[] buttonClick;
    [SerializeField] AudioClip[] badSelection;
    [SerializeField] AudioClip[] menuOpen;
    [SerializeField] AudioClip[] menuClose;
    [SerializeField] AudioClip[] constructionComplete;

    [Header("Ambient Clips")]
    [SerializeField] AudioClip[] homeAmbient;
    [SerializeField] AudioClip[] farmWildAmbient;

    [Header("Music Clips")]
    [SerializeField] AudioClip[] homeMusic;
    [SerializeField] AudioClip[] farmMusic;
    #endregion

    float updateInterval = 1;
    float lastUpdateTime;
    Transform _player;
    Transform _child;

    public AudioClip[] GetClips(ESoundType soundType)
    {
        string key = soundType.ToString();

        if (AudioDictionary.TryGetValue(key, out AudioClip[] clips))
        {
            return clips;
        }
        else
        {
            Debug.LogWarning($"AudioClip[] not found for sound type: {key}");
            return null;
        }
    }

    private void Awake()
    {
        InstantiateAudioSources();
        InitializeClipDictionary();
    }
    private void InstantiateAudioSources()
    {
        _child = new GameObject("AudioSources").transform;
        audioSourcePools = new Dictionary<ESoundSource, List<AudioSource>>();

        foreach (ESoundSource soundSource in Enum.GetValues(typeof(ESoundSource)))
        {
            audioSourcePools.Add(soundSource, new List<AudioSource>());
        }

        foreach (SoundSource source in soundSources.ToList())
        {
            for (int i = 0; i < source.instances; i++)
            {
                AudioSource audioSource = _child.gameObject.AddComponent<AudioSource>();
                audioSourcePools[source.Source].Add(audioSource);
            }
        }
    }
    void InitializeClipDictionary()
    {
        Type audioManagerType = typeof(AudioManager); // Get the type of the AudioManager class
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        foreach (ESoundType soundType in Enum.GetValues(typeof(ESoundType)))
        {
            string soundTypeName = soundType.ToString();

            // Use reflection to find the field that matches the enum name
            FieldInfo fieldInfo = audioManagerType.GetField(soundTypeName, bindingFlags);

            if (fieldInfo != null && fieldInfo.FieldType == typeof(AudioClip[]))
            {
                AudioClip[] clips = (AudioClip[])fieldInfo.GetValue(this); // Get the value of the field

                if (clips != null)
                {
                    AudioDictionary.Add(soundTypeName, clips);
                }
                else
                {
                    Debug.LogWarning($"AudioClip[] for {soundTypeName} is null.");
                }
            }
            else
            {
                Debug.LogWarning($"Field not found for sound type: {soundTypeName}");
            }
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.onPlaySound += OnAudioPlay;
    }
    private void OnDisable()
    {
        GameManager.Instance.onPlaySound += OnAudioPlay;
    }
    private void Start()
    {
        _player = GameManager.Instance.Player;
        lastUpdateTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            lastUpdateTime = Time.time;
            UpdateAudioSourcePriorities();
        }
    }
    void UpdateAudioSourcePriorities()
    {

        Vector3 playerPosition = _player.transform.position;
        foreach (var key in audioSourcePools.Keys.ToList())
        {
            AssignAudioSourcesToChannels(audioSourcePools[key], playerPosition);
        }
    }
    void AssignAudioSourcesToChannels(List<AudioSource> audioSources, Vector3 playerPosition)
    {
        List<(AudioSource source, float distance)> sourceDistances = new List<(AudioSource, float)>();

        foreach (AudioSource source in audioSources)
        {
            float distance = Vector3.Distance(playerPosition, source.transform.position);
            sourceDistances.Add((source, distance));
        }

        sourceDistances.Sort((pair1, pair2) => pair1.distance.CompareTo(pair2.distance));

        for (int i = 0; i < sourceDistances.Count && i < audioSourceCountPerType; i++)
        {
            AssignToChannel(sourceDistances[i].source, i);
        }
    }
    void AssignToChannel(AudioSource source, int channel)
    {
        source.priority = channel;
    }
    public void OnAudioPlay(SoundRequest request)
    {
        var availableAudioSources = audioSourcePools[request.SoundSource].Where(a => !a.isPlaying).ToList();
        if (availableAudioSources.Any())
        {
            AudioSource audioSource = availableAudioSources.First();
            if (request.RandomizePitch)
                RandomizePitch(audioSource);
            if (request.Loop)
                audioSource.loop = request.Loop;

            var clips = GetClips(request.SoundType);
            if (clips == null)
            {
                Debug.LogError($"No clips found for sound type: {request.SoundType}");
                return;
            }
            audioSource.clip = RandomizeClip(clips);
            audioSource.Play();
        }
    }

    public AudioClip RandomizeClip(AudioClip[] clips)
    {
        if (clips.Length == 0)
        {
            Debug.LogError("No clips found");
            return null;
        }
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
    void RandomizePitch(AudioSource source)
    {
        source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
    }
}
