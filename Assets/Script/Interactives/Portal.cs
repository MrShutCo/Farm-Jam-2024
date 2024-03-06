using System.Collections;
using System.Collections.Generic;
using Assets.Script.UI;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public event System.Action onPortalComplete;
    enum PortalDestinationType
    {
        Specify,
        Home
    }
    [SerializeField] PortalDestinationType destinationType;
    [SerializeField] Portal destination;
    [SerializeField] bool isEntrance;
    [SerializeField] bool deactivate = false;
    [SerializeField] bool destroyDestinationOnComplete = false;
    [SerializeField] EGameState switchStateOnComplete;
    [SerializeField] bool wildPortal;
    SpriteRenderer spriteRenderer;
    Vector2 offset = new Vector2(0, -.5f);
    private DialogueText choosePortalLocation;

    SoundRequest enterPortalSound;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (wildPortal) spriteRenderer.enabled = false;
        enterPortalSound = new SoundRequest
        {
            SoundSource = ESoundSource.Player,
            RequestingObject = gameObject,
            SoundType = ESoundType.playerEnterPortal,
            RandomizePitch = true,
            Loop = false
        };
    }

    private void OnEnable()
    {
        if (destinationType == PortalDestinationType.Home)
        {
            SetDestination(GameObject.Find("Portal-Home").GetComponent<Portal>());
        }
    }
    private void OnDisable()
    {
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (deactivate) return;
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                if (GameManager.Instance.GameState == EGameState.Wild)
                {
                    onChoose(other, "");
                    return;
                }
                CreateDialogue(other);
                GameManager.Instance.SetGameState(EGameState.Dialogue);
                DialogueManager.Instance.StartDialogue(choosePortalLocation);
            }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                Activate();
                // TODO: this isnt right. If this is set to true, and the 
                DialogueManager.Instance.EndDialogue(false);
            }
    }

    void CreateDialogue(Collider2D other)
    {
        int progressIndex = GameManager.Instance.ProgressManager.ProgressIndex;
        List<DialogueOption> locations = new List<DialogueOption>(); 
        locations.Add(new DialogueOption("Farm", new DialogueAction("Teleporting...", () => onChoose(other,"Farm"))));
        if (progressIndex >= 2)
            locations.Add(new DialogueOption("Industrial Block", new DialogueAction("Teleporting...", () => onChoose(other,"Industrial Block"))));
        if (progressIndex >= 4)
            locations.Add(new DialogueOption("City", new DialogueAction("Teleporting...", () => onChoose(other,"City"))));
        choosePortalLocation = new DialogueText("Where would you like to teleport to", locations);
    }

    void onChoose(Collider2D other, string location)
    {
        GameManager.Instance.ChosenWorld = location;
        Debug.Log("Player at portal");
        var player = other.GetComponent<Rigidbody2D>();
        GameManager.Instance.onPlaySound?.Invoke(enterPortalSound);
        GameManager.Instance.onTeleport?.Invoke(true, (Vector2)destination.transform.position + offset);
        destination.Deactivate();
        player.simulated = false;
        player.transform.position = destination.transform.position;
        player.simulated = true;
        GameManager.Instance.onTeleport?.Invoke(false, (Vector2)destination.transform.position + offset);
        GameManager.Instance.SetGameState(EGameState.Normal);
        GameManager.Instance.SetGameState(switchStateOnComplete);

        onPortalComplete?.Invoke();
    }

    public void SetDestination(Portal portal)
    {
        destination = portal;
    }
    public void Deactivate()
    {
        deactivate = true;
    }
    public void Activate()
    {
        deactivate = false;
    }
}
