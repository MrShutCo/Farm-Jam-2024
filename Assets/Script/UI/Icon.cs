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
    [SerializeField] Sprite otherIcon; //if not null, this will be used instead of the resource icon
    [SerializeField] Sprite woodIcon;
    [SerializeField] Sprite steelIcon;
    [SerializeField] Sprite electronicsIcon;
    [SerializeField] Sprite foodIcon;
    [SerializeField] Sprite bloodIcon;
    [SerializeField] Sprite boneIcon;
    [SerializeField] Sprite organIcon;
    public EResource assignedResource { get; private set; }

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
        if (otherIcon != null)
        {
            SetIcon(otherIcon, 0, false);
        }
    }

    /// <summary>
    /// Set the icon to the specified resource and quantity
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="qty"></param>
    /// <param name="showQty"></param>
    public void SetIcon(EResource resource, int qty, bool showQty = true)
    {
        // Set the sprite based on the resource using the dictionary
        spriteRenderer.sprite = resourceSprites[resource];
        assignedResource = resource;
        if (showQty)
        {
            text.text = qty.ToString();
        }
        else
        {
            text.text = "";
        }
    }

    /// <summary>
    /// Set the icon to a custom sprite and quantity
    /// </summary>
    /// <param name="customSprite"></param>
    /// <param name="qty"></param>
    /// <param name="showQty"></param>
    public void SetIcon(Sprite customSprite, int qty, bool showQty = true)
    {
        spriteRenderer.sprite = customSprite;
        if (showQty)
        {
            text.text = qty.ToString();
        }
        else
        {
            text.text = "";
        }
    }
}

