using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI textLabel;
    [SerializeField] float delayBetweenCharacters;
    [SerializeField] List<RectTransform> options;
    Coroutine typeLineCR;

    private Queue<string> dialogue = new Queue<string>();

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        foreach (RectTransform option in options)
        {
            option.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (GameManager.Instance.GameState == EGameState.Dialogue)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space pressed");
                if (typeLineCR == null)
                    DisplayNextLine();
            }

    }


    /// Convert to use Scriptable objects DialogueLines[]> 
    //Dialogue line to include string, dialogue options, ability to trigger upgrades, feed husband, etc)
    public void StartDialogue(string[] dialogue)
    {
        Debug.Log("Starting dialogue");
        this.dialogue.Clear();
        foreach (string line in dialogue)
        {
            Debug.Log("Adding line: " + line);
            this.dialogue.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        Debug.Log("Displaying next line");
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
        Debug.Log("Typing line: " + line);
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
        Debug.Log("Ending dialogue ");
        GameManager.Instance.SetGameState(EGameState.Normal);
    }

}