using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum EResource
{
    Blood,
    Bones,
    Brains,
    Wood,
    Metal,
    Electronics
}

public class Farm : MonoBehaviour
{
    Dictionary<EResource, Color> ResourceColors = new Dictionary<EResource, Color>()
    {
        { EResource.Blood, Color.red }, { EResource.Wood, From255(0x66,0x33,0) },{ EResource.Bones, From255(249,246,238) },
        { EResource.Electronics, Color.yellow }, { EResource.Metal, From255(0xAA, 0xA9, 0xAD) },{ EResource.Brains, From255(0xd9, 0xa5, 0xb2) },
    };

    public int NumberOfPeople;
    public EResource Resource;
    public float TimeToMaturity;
    float _maturityLevel;

    FloatingStatusBar _harvestBar;

    // Start is called before the first frame update
    void Start()
    {
       var renderer = GetComponent<SpriteRenderer>();
       renderer.color = ResourceColors[Resource];
        _maturityLevel = 0;
        _harvestBar = GetComponentInChildren<FloatingStatusBar>();
    }

    // Update is called once per frame
    void Update()
    {
        _maturityLevel += Time.deltaTime;
        _maturityLevel = Mathf.Clamp(_maturityLevel, 0, TimeToMaturity);
        _harvestBar.UpdateHealthBar(_maturityLevel, TimeToMaturity);
    }

    static Color From255(int r, int g, int b)
    => new Color(r / 256.0f, g / 256.0f, b / 256.0f);
}
