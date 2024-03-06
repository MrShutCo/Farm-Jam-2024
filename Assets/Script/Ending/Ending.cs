using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] SpriteRenderer Earth;
    [SerializeField] SpriteRenderer Husband;
    [SerializeField] SpriteRenderer Hat;

    [SerializeField] Color finalEarthColor;
    [SerializeField] Color finalHusbandColor;
    [SerializeField] SpriteRenderer endText;

    private void Start()
    {
        GameManager.Instance.gameObject.SetActive(false);
        StartCoroutine(EndSequence());
    }
    IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(LerpToColor(Earth, finalEarthColor, 3));
        yield return new WaitForSeconds(.25f);
        StartCoroutine(LerpToPosition(Husband.transform, new Vector3(Husband.transform.position.x, -0.57f, 0), 3));
        GetComponent<AudioSource>().Play();
        StartCoroutine(LerpToVolume(GetComponent<AudioSource>(), 0.45f, 3));
        StartCoroutine(LerpToColor(Hat, finalHusbandColor, 3));
        yield return StartCoroutine(LerpToColor(Husband, finalHusbandColor, 3));
        yield return new WaitForSeconds(.25f);
        yield return StartCoroutine(LerpToColor(endText, Color.white, 3));
        // Load next scene
    }
    IEnumerator LerpToVolume(AudioSource audioSource, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = audioSource.volume;

        while (time < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    IEnumerator LerpToColor(SpriteRenderer sprite, Color targetColor, float duration)
    {
        float time = 0;
        Color startColor = sprite.color;

        while (time < duration)
        {
            sprite.color = Color.Lerp(startColor, targetColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        sprite.color = targetColor;
    }
    IEnumerator LerpToPosition(Transform transform, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
