using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenu : MonoBehaviour
{

    [SerializeField] Button startButton;
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] Image startPanel;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    bool startButtonFadeActive = false;
    private void Awake()
    {

        if (startText != null)
            startText.canvasRenderer.SetAlpha(0);
        startPanel.canvasRenderer.SetAlpha(1);
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
    }
    private void Start()
    {
        StartCoroutine(StartUpSequence());
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
                if (!startButtonFadeActive)
                {
                    startButtonFadeActive = true;
                    StartCoroutine(StartButtonFadeInOut());
                }
                else
                    StartGame();
            }
    }
    void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    IEnumerator StartUpSequence()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PanelFadeOut());
        yield return StartCoroutine(BloodAndSound());
        if (!startButtonFadeActive)
            yield return StartCoroutine(StartButtonFadeInOut());
    }
    IEnumerator PanelFadeOut()
    {
        startPanel.CrossFadeAlpha(0, 2, false);
        yield return new WaitForSeconds(2);
    }
    IEnumerator PanelFadeIn()
    {
        startPanel.CrossFadeAlpha(1, 2, false);
        yield return new WaitForSeconds(2);
    }
    IEnumerator BloodAndSound()
    {

        //Play Slash Sound
        yield return new WaitForSeconds(2.25f);
        //Play 2 Slashes
        yield return new WaitForSeconds(2.5f);
        //Play juicy slash sound
        yield return new WaitForSeconds(1.5f);
        //Play loud percussion;
        //Play Music
    }
    IEnumerator StartButtonFadeInOut()
    {
        startButtonFadeActive = true;
        if (startText != null)
            while (true)
            {
                startText.CrossFadeAlpha(1, 1, false);
                yield return new WaitForSeconds(1);
                startText.CrossFadeAlpha(0, 1, false);
                yield return new WaitForSeconds(1);

            }
    }

    IEnumerator StartButtonPressed()
    {
        StartCoroutine(FadeOutSoundAndMusic());
        yield return StartCoroutine(PanelFadeIn());

        StartGame();
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
