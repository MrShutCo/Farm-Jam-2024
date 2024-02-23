using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    TextMeshProUGUI text;
    [SerializeField] Sprite woodIcon;
    [SerializeField] Sprite steelIcon;
    [SerializeField] Sprite electronicsIcon;
    [SerializeField] Sprite foodIcon;
    [SerializeField] Sprite bloodIcon;
    [SerializeField] Sprite boneIcon;
    [SerializeField] Sprite organIcon;

    private Dictionary<EResource, Sprite> resourceSprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        // Initialize the dictionary with the mappings
        resourceSprites = new Dictionary<EResource, Sprite>
            {
                { EResource.Wood, woodIcon },
                { EResource.Steel, steelIcon },
                { EResource.Electronics, electronicsIcon },
                { EResource.Food, foodIcon },
                { EResource.Blood, bloodIcon },
                { EResource.Bones, boneIcon },
                { EResource.Organs, organIcon }
            };
    }

    public void SetIcon(EResource resource, int qty)
    {
        // Set the sprite based on the resource using the dictionary
        spriteRenderer.sprite = resourceSprites[resource];
        text.text = qty.ToString();
    }
    public void SetIcon(Sprite customSprite, int qty)
    {
        spriteRenderer.sprite = customSprite;
        text.text = qty.ToString();
    }
}

