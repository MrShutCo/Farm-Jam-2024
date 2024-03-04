using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] Sprite BackgroundHome;
    [SerializeField] Vector2 HomePos = new Vector2(0, 0);
    [SerializeField] Sprite BackgroundWild;
    [SerializeField] Vector2 WildPos = new Vector2(-220, 0);
    private Material _material;
    [SerializeField] private float speed;

    private float currentscroll = 0;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChange += ChangeBackground;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChange -= ChangeBackground;
        GameManager.Instance.onGameStateChange -= ChangeBackground;
    }
    void ChangeBackground(EGameState gameState)
    {
        Debug.Log("Changing background");
        if (gameState == EGameState.Normal)
        {
            transform.position = HomePos;
            GetComponent<SpriteRenderer>().sprite = BackgroundHome;
        }
        else if (gameState == EGameState.Wild)
        {
            Debug.Log("Changing sprite background");
            transform.position = WildPos;
            GetComponent<SpriteRenderer>().sprite = BackgroundWild;
        }
    }

    void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        currentscroll += speed * Time.deltaTime;
        _material.mainTextureOffset = new Vector2(currentscroll, 0);
    }
}
