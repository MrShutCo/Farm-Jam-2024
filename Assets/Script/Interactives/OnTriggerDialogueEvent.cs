using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.UI;
using UnityEngine;

public class OnTriggerDialogueEvent : MonoBehaviour
{
    [SerializeField] DialogueText dialogue;

    private void OnEnable()
    {
        Func<bool> noPred = () => true;
        dialogue = new DialogueText("(What would you like to do?)", new ()
        {
            new ("Talk", new TalkToHusband()),
            new ("Build", new BuildDialogue()), 
            new ("Human Sacrifice", new DialogueText("Get sacrificed scrub", null)),
            new ("Resource Sacrifice", new FeedHusband()),
            new ("Upgrade",new DialogueUpgrade())
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetGameState(EGameState.Dialogue);
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
