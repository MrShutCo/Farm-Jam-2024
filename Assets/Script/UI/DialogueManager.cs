using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMPro.TextMeshProUGUI textLabel;
    [SerializeField] private float delayBetweenCharacters;
    public bool IsDialogueActive => dialogueBox.activeInHierarchy;

    Coroutine typeLineCR;

    private Queue<string> dialogue = new Queue<string>();

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (dialogueBox.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        {
            if (typeLineCR == null)
                DisplayNextLine();
        }
    }

    public void StartDialogue(string[] dialogue)
    {
        Time.timeScale = 0;
        dialogueBox.SetActive(true);
        this.dialogue.Clear();
        foreach (string line in dialogue)
        {
            this.dialogue.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogue.Count == 0)
        {
            EndDialogue();
            return;
        }
        StopAllCoroutines();
        typeLineCR = StartCoroutine(TypeLine(dialogue.Dequeue()));
    }

    IEnumerator TypeLine(string line)
    {
        textLabel.text = "";
        foreach (char c in line.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                textLabel.text = line;
                break;
            }
            textLabel.text += c;
            yield return new WaitForSecondsRealtime(delayBetweenCharacters);
        }
        typeLineCR = null;
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);
        Time.timeScale = 1;
    }

}