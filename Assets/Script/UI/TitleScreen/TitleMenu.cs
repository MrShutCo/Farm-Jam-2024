using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenu : MonoBehaviour
{

    [SerializeField] Button startButton;
    [SerializeField] SpriteRenderer startText;
    [SerializeField] Image startPanel;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    bool startButtonFadeActive = false;
    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(StartGame);
    }

    private void Update()
    {
        if (startButton != null)
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }
    }
    void StartGame()
    {
        SceneManager.LoadScene(1);
    }


    IEnumerator FadeOutSoundAndMusic()
    {
        //Fade out music
        float volume = sfxSource.volume;
        float musicVolume = musicSource.volume;
        while (volume > 0 || musicVolume > 0)
        {
            if (volume > 0)
            {
                volume = Mathf.Lerp(volume, 0, 0.1f);
                sfxSource.volume = volume;
            }
            if (musicVolume > 0)
            {
                musicVolume = Mathf.Lerp(musicVolume, 0, 0.1f);
                musicSource.volume = musicVolume;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
    }
}
