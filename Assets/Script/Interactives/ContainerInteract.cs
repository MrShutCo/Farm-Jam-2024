using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInteract : Interactable
{
    bool opened;

    public override void Interact(Player player)
    {
        if (!opened)
        {
            opened = true;
            Destroy(gameObject);
        }
    }
}
