using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDialogueEvent : MonoBehaviour
{
    [SerializeField] string[] dialogueTest;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetGameState(EGameState.Dialogue);
            DialogueManager.Instance.StartDialogue(dialogueTest);
        }
    }
}
