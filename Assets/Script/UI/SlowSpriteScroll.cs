using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpriteScroll : MonoBehaviour
{
    Renderer _renderer;
    [SerializeField] private float speed;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (_renderer.material != null)
        {
            float currentscroll = _renderer.material.mainTextureOffset.x;
            float vertical = _renderer.material.mainTextureOffset.y;
            vertical += Mathf.Cos(Time.time * speed * 5) * Time.deltaTime * .025f;
            currentscroll += speed * Time.deltaTime;
            _renderer.material.mainTextureOffset = new Vector2(currentscroll, vertical);
        }
    }
}
