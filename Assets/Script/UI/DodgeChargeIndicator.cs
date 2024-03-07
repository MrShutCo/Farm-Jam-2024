using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeChargeIndicator : MonoBehaviour
{
    [SerializeField] Icon icon;
    [SerializeField] Sprite dodgeIcon;
    Player player;
    List<Icon> iconsList = new();
    int _currentCharges = 0;

    private void Awake()
    {
        player = GameManager.Instance.Player.GetComponent<Player>();
    }
    private void OnEnable()
    {
        player.onUpdateMaxDodgeCharges += UpdateMaxCharges;
        player.dodgeAction.onUpdateCurrentCharges += UpdateCurrentCharges;
    }
    private void OnDisable()
    {
        player.onUpdateMaxDodgeCharges -= UpdateMaxCharges;
        player.dodgeAction.onUpdateCurrentCharges -= UpdateCurrentCharges;
    }
    void UpdateMaxCharges(int maxCharges)
    {
        int maxDiff = maxCharges - iconsList.Count;
        if (maxDiff < 0)
        {
            for (int i = iconsList.Count; i > maxCharges; i--)
            {
                Destroy(iconsList[i]);
                iconsList.RemoveAt(i);
            }
        }
        else if (maxDiff > 0)
        {
            for (int i = iconsList.Count; i < maxDiff; i++)
            {
                Icon newIcon = Instantiate(icon, transform.position + new Vector3(18f * i, 0, 0), Quaternion.identity, transform);
                iconsList.Add(newIcon);
                newIcon.SetIcon(dodgeIcon, 0, false);
            }
        }
    }
    void UpdateCurrentCharges(int currentCharges)
    {

        if (_currentCharges != currentCharges)
        {
            int currentDiff = currentCharges - _currentCharges;
            if (currentDiff < 0)
            {
                for (int i = _currentCharges - 1; i >= currentCharges; i--)
                {
                    iconsList[i].gameObject.SetActive(false);
                }
            }
            else if (currentDiff > 0)
            {
                for (int i = _currentCharges; i < currentCharges; i++)
                {
                    if (i < iconsList.Count)
                    {
                        iconsList[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        Icon newIcon = Instantiate(icon, transform.position + new Vector3(0.5f * i, 0, 0), Quaternion.identity, transform);
                        iconsList.Add(newIcon);
                        newIcon.SetIcon(dodgeIcon, 0, false);
                    }
                }
            }
            _currentCharges = currentCharges;
        }

    }
}
