// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using System;



// public class AudioManager2 : MonoBehaviour
// {
//     Transform player;
//     AudioManager audioManager;
//     public int audioSourceCountPerType = 4;
//     private Dictionary<ESoundSource, List<AudioSource>> audioSourcePools;

//     float updateInterval = 1;
//     float updateTimer;

//     private void OnEnable()
//     {
//         GameManager.Instance.onPlaySound += OnAudioPlay;

//     }
//     private void OnDisable()
//     {
//         GameManager.Instance.onPlaySound -= OnAudioPlay;
//     }

//     private void Start()
//     {
//         player = GameManager.Instance.Player;
//         audioManager = GetComponent<AudioManager>();
//         audioSourcePools = new Dictionary<ESoundSource, List<AudioSource>>();
//         foreach (ESoundSource soundSource in Enum.GetValues(typeof(ESoundSource)))
//         {
//             audioSourcePools.Add(soundSource, new List<AudioSource>());
//         }

//         foreach (var key in audioSourcePools.Keys.ToList())
//         {
//             for (int i = 0; i < audioSourceCountPerType; i++)
//             {
//                 AudioSource audioSource = gameObject.AddComponent<AudioSource>();
//                 audioSourcePools[key].Add(audioSource);
//             }
//         }
//     }


//     private void Update()
//     {
//         if (Time.time - updateTimer > updateInterval)
//         {
//             updateTimer = Time.time;
//             UpdateAudioSourcePriorities();
//         }
//     }

//     void UpdateAudioSourcePriorities()
//     {

//         Vector3 playerPosition = player.transform.position;
//         foreach (var key in audioSourcePools.Keys.ToList())
//         {
//             AssignAudioSourcesToChannels(audioSourcePools[key], playerPosition);
//         }
//     }
//     void AssignAudioSourcesToChannels(List<AudioSource> audioSources, Vector3 playerPosition)
//     {
//         // Implement your logic to prioritize and assign audio sources based on proximity
//         List<(AudioSource source, float distance)> sourceDistances = new List<(AudioSource, float)>();

//         foreach (AudioSource source in audioSources)
//         {
//             float distance = Vector3.Distance(playerPosition, source.transform.position);
//             sourceDistances.Add((source, distance));
//         }

//         sourceDistances.Sort((pair1, pair2) => pair1.distance.CompareTo(pair2.distance));

//         for (int i = 0; i < sourceDistances.Count && i < audioSourceCountPerType; i++)
//         {
//             AssignToChannel(sourceDistances[i].source, i);
//         }
//     }
//     void AssignToChannel(AudioSource source, int channel)
//     {
//         source.priority = channel;
//     }

//     public void OnAudioPlay(SoundRequest request)
//     {
//         Debug.Log("On Audio Play Sound Request: " + request.soundType + " " + request.soundSource + " " + request.requestingObject.name);

//         var availableAudioSources = audioSourcePools[request.soundSource].Where(a => !a.isPlaying).ToList();
//         Debug.Log("Are there any available audio sources: " + availableAudioSources.Any());

//         if (availableAudioSources.Any())
//         {
//             AudioSource audioSource = availableAudioSources.First();
//             Debug.Log("AudioSource: " + audioSource.name);
//             Debug.Log("Clips: " + audioManager.GetClips(request.soundType).Length);
//             Debug.Log("Randomized Clip: " + audioManager.RandomizeClip(audioManager.GetClips(request.soundType)));

//             audioSource.clip = audioManager.RandomizeClip(audioManager.GetClips(request.soundType));
//             audioSource.Play();
//         }
//     }
// }