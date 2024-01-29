using System.Collections;
using System.Collections.Generic;
using Assets.Script.Player;
using UnityEngine;

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
        foreach(var c in colliders)
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
