using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.UI;
using UnityEngine;

public class OnTriggerDialogueEvent : MonoBehaviour
{
    [SerializeField] DialogueText dialogue;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    
    private SpriteRenderer _spriteRenderer;
    
    private void OnEnable()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        dialogue = new DialogueText("(What would you like to do?)", new ()
        {
            new ("Talk", new TalkToHusband()),
            new ("Build", new BuildDialogue()), 
            new ("Resource Sacrifice", new FeedHusband()),
            new ("Upgrade Buildings",new DialogueUpgrade())
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetGameState(EGameState.Dialogue);
            DialogueManager.Instance.StartDialogue(dialogue);
            _spriteRenderer.sprite = activeSprite;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.EndDialogue(true);
            _spriteRenderer.sprite = inactiveSprite;
        }
    }
}
