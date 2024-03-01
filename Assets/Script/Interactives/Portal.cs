using System.Collections;
using System.Collections.Generic;
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

    SoundRequest enterPortalSound;
    private void Awake()
    {
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (deactivate) return;
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player at portal");
                var player = other.GetComponent<Rigidbody2D>();
                GameManager.Instance.onPlaySound?.Invoke(enterPortalSound);
                GameManager.Instance.onTeleport?.Invoke(true, destination.transform.position);
                destination.Deactivate();
                player.simulated = false;
                player.transform.position = destination.transform.position;
                player.simulated = true;
                GameManager.Instance.onTeleport?.Invoke(false, destination.transform.position);
                onPortalComplete?.Invoke();
                GameManager.Instance.SetGameState(switchStateOnComplete);
            }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isEntrance)
            if (other.CompareTag("Player"))
            {
                Activate();
            }
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
