using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] Sprite BackgroundHome;
    [SerializeField] Vector2 HomePos = new Vector2(0, 0);
    private Material _material;
    [SerializeField] private float speed;

    private float currentscroll = 0;

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

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
