using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public enum ESoundType
{
    playerMelee,
    playerDeath,
    playerEnterPortal,
    humanMelee,
    humanShotgun,
    humanDeath,
    meleeImpact,
    buildingImpact,
    buildingDeath,
    buildingConstructed,
    buttonClick,
    menuOpen,
    menuClose,
    homeAmbient,
    farmWildAmbient,
    homeMusic,
    farmMusic
}
[System.Serializable]
public class AudioManager : MonoBehaviour
{
    [SerializeField] public Dictionary<string, AudioClip[]> AudioDictionary = new();
    public PriorityQueue<Vector3, float> humanSoundQueue = new PriorityQueue<Vector3, float>(0);

    #region AudioSources
    AudioSource[] humanSources;
    AudioSource[] buildingSources;
    AudioSource ui;
    AudioSource ambient;
    AudioSource music;
    AudioSource husband;
    AudioSource player;
    #endregion

    #region  AudioClips
    [Header("Player Clips")]
    [SerializeField] AudioClip[] playerMelee;
    [SerializeField] AudioClip[] playerDeath;
    [SerializeField] AudioClip[] playerEnterPortal;

    [Header("Human Clips")]
    [SerializeField] AudioClip[] humanMelee;
    [SerializeField] AudioClip[] humanShotgun;
    [SerializeField] AudioClip[] humanDeath;

    [Header("General Clips")]
    [SerializeField] AudioClip[] meleeImpact;

    [Header("Building Clips")]
    [SerializeField] AudioClip[] buildingImpact;
    [SerializeField] AudioClip[] buildingDeath;
    [SerializeField] AudioClip[] buildingConstructed;

    [Header("UI Clips")]
    [SerializeField] AudioClip[] buttonClick;
    [SerializeField] AudioClip[] menuOpen;
    [SerializeField] AudioClip[] menuClose;

    [Header("Ambient Clips")]
    [SerializeField] AudioClip[] homeAmbient;
    [SerializeField] AudioClip[] farmWildAmbient;

    [Header("Music Clips")]
    [SerializeField] AudioClip[] homeMusic;
    [SerializeField] AudioClip[] farmMusic;
    #endregion



    private void Awake()
    {
        InstantiateAudioSources();
        InitializeClipDictionary();
    }
    void InitializeClipDictionary()
    {
        for (int i = 0; i < ESoundType.GetValues(typeof(ESoundType)).Length; i++)
        {
            string soundTypeName = ESoundType.GetValues(typeof(ESoundType)).GetValue(i).ToString();
            AudioDictionary.Add(soundTypeName, GetClips((ESoundType)Enum.Parse(typeof(ESoundType), soundTypeName)));
        }
    }
    // private AudioClip[] GetPropertyValue(ESoundType eSoundType, string propName)
    // {
    //     return eSoundType.GetType().GetProperty(propName).GetValue(eSoundType, null);
    // }
    // private T GetPropertyValue<T>(object obj, string propName)
    // {
    //     return (T)obj.GetType().GetProperty(propName).GetValue(obj, null);
    // }

    private void InstantiateAudioSources()
    {
        humanSources = new AudioSource[5];
        buildingSources = new AudioSource[5];
        for (int i = 0; i < 5; i++)
        {
            humanSources[i] = gameObject.AddComponent<AudioSource>();
            buildingSources[i] = gameObject.AddComponent<AudioSource>();
        }
        ui = Camera.main.gameObject.AddComponent<AudioSource>();
        ambient = Camera.main.gameObject.AddComponent<AudioSource>();
        music = Camera.main.gameObject.AddComponent<AudioSource>();
        husband = gameObject.AddComponent<AudioSource>();
        player = GameManager.Instance.Player.gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameManager.Instance.onPlayPlayerSound += PlayPlayerSound;
    }
    private void OnDisable()
    {
        GameManager.Instance.onPlayPlayerSound -= PlayPlayerSound;
    }

    AudioClip RandomizeClip(AudioClip[] clips)
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
    void RandomizePitch(AudioSource source)
    {
        source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
    }
    AudioClip[] GetClips(ESoundType soundType)
    {
        return soundType switch
        {
            ESoundType.playerMelee => playerMelee,
            ESoundType.playerDeath => playerDeath,
            ESoundType.playerEnterPortal => playerEnterPortal,
            ESoundType.humanMelee => humanMelee,
            ESoundType.humanShotgun => humanShotgun,
            ESoundType.humanDeath => humanDeath,
            ESoundType.meleeImpact => meleeImpact,
            ESoundType.buildingImpact => buildingImpact,
            ESoundType.buildingDeath => buildingDeath,
            ESoundType.buildingConstructed => buildingConstructed,
            ESoundType.buttonClick => buttonClick,
            ESoundType.menuOpen => menuOpen,
            ESoundType.menuClose => menuClose,
            ESoundType.homeAmbient => homeAmbient,
            ESoundType.farmWildAmbient => farmWildAmbient,
            ESoundType.homeMusic => homeMusic,
            ESoundType.farmMusic => farmMusic,
            _ => null,
        };
    }

    public void PlayHumanSound(ESoundType soundType, Vector2 source)
    {
        for (int i = 0; i < humanSources.Length; i++)
        {
            if (!humanSources[i].isPlaying)
            {
                RandomizePitch(humanSources[i]);
                humanSources[i].transform.position = source;
                humanSources[i].clip = RandomizeClip(AudioDictionary[soundType.ToString()]);
                humanSources[i].Play();
                return;
            }
        }
    }
    public void PlayBuildingSound(ESoundType soundType, Vector2 source)
    {
        for (int i = 0; i < buildingSources.Length; i++)
        {
            if (!buildingSources[i].isPlaying)
            {
                RandomizePitch(buildingSources[i]);
                buildingSources[i].transform.position = source;
                buildingSources[i].clip = RandomizeClip(AudioDictionary[soundType.ToString()]);
                buildingSources[i].Play();
                Debug.Log("PLayed");
                return;
            }
        }
    }

    public void PlayPlayerSound(ESoundType soundType)
    {
        RandomizePitch(player);
        player.clip = RandomizeClip(AudioDictionary[soundType.ToString()]);
        player.Play();
    }
    public void PlayUISound(ESoundType soundType, Vector2 source)
    {
        RandomizePitch(ui);
        ui.clip = RandomizeClip(AudioDictionary[soundType.ToString()]);
        ui.Play();

    }
    public void PlayMusic(ESoundType soundType, Vector2 source = default)
    {
        music.clip = RandomizeClip(AudioDictionary[soundType.ToString()]);
        music.Play();
    }
}
