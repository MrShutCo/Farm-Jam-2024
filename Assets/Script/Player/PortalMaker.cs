using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMaker : MonoBehaviour
{

    [SerializeField] Color targetColor;
    Collider2D col;
    Transform graphics;
    SpriteRenderer spriteRenderer;
    Color baseColor;
    Portal portal;
    float timeToGrow = 1;

    Vector3 maxScale = new Vector3(1, 1, 1);

    private void Awake()
    {
        graphics = transform.GetChild(0);
        spriteRenderer = graphics.GetComponent<SpriteRenderer>();
        col = graphics.GetComponent<Collider2D>();
        portal = graphics.GetComponent<Portal>();
        this.enabled = false;
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
        graphics.gameObject.SetActive(true);
        portal.onPortalComplete += () => Disable();
        baseColor = graphics.GetComponent<SpriteRenderer>().color;
        StartCoroutine(LerpScale());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        graphics.transform.localScale = new Vector3(0, 0, 0);
        spriteRenderer.color = baseColor;
        portal.onPortalComplete -= () => Disable();

    }

    private void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        graphics.transform.Rotate(Vector3.forward * 180 * 8 * Time.deltaTime);
    }

    IEnumerator LerpScale()
    {
        //lerp using the timeToGrow

        float t = 0;
        while (t < timeToGrow)
        {
            t += Time.deltaTime;
            float normalizedT = t / timeToGrow;
            if (normalizedT > 0.95f)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, (normalizedT - .95f) * 10);
            }
            graphics.transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, normalizedT);
            yield return null;
        }
        graphics.transform.localScale = maxScale;
        spriteRenderer.color = targetColor;
        col.enabled = true;
        portal.transform.SetParent(null);
    }

    void Disable()
    {
        col.enabled = false;
        portal.transform.parent = this.transform;
        portal.transform.localPosition = Vector3.zero;
        graphics.gameObject.SetActive(false);
        this.enabled = false;
    }
}
