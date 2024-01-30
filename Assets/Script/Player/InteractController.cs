using System;
using Assets.Script.Player;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    Player player;
    [SerializeField] float offsetDistance = 1f;
    [SerializeField] float sizeOfInteracatableArea = 1.2f;
    [SerializeReference] HighlightController highlightController;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();

    }

    private void Update()
    {
        Check();

        if (Input.GetMouseButtonDown(1))
        {
            Check<Interactable>(t => t.Interact(player));
        }
    }

    private void Check()
    {
        if(!Check<Interactable>(t => highlightController.Highlight(t.gameObject)))
        {
            highlightController.Hide();
        }
    }

    public bool Check<T>(Action<T> onCheck)
    {
        Vector2 position = rigidbody.position + player.Facing * offsetDistance;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, sizeOfInteracatableArea);
        foreach (var c in colliders)
        {
            var hit = c.GetComponent<T>();
            if (hit != null)
            {
                onCheck(hit);
                return true;
            }
        }
        return false;
    }
}

public class ToolsCharacterController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    Player player;
    [SerializeField] float offsetDistance = 1f;
    [SerializeField] float sizeOfInteracatableArea = 1.2f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UseTool();
        }
    }

    private void UseTool()
    {
        Vector2 position = rigidbody.position + player.Facing * offsetDistance;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, sizeOfInteracatableArea);
        foreach (var c in colliders)
        {
            var hit = c.GetComponent<ToolHit>();
            if (hit != null)
            {
                hit.Hit(this);
                break;
            }
        }
    }
}
