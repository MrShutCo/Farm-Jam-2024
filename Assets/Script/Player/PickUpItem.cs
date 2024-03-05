using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using Unity.Mathematics;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] float timeToLive = 10f;
    [SerializeField] ResourceSO resource;
    Carrier carrier;
    SpriteRenderer spriteRenderer;
    bool pickUp = false;
    void Start()
    {
        player = GameManager.Instance.Player;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = resource.ResourceSprite;
    }

    public void PickUp(Carrier carrier)
    {
        this.carrier = carrier;
        pickUp = true;
    }

    void Update()
    {
        if (!pickUp)
        {
            //float up and down slowly
            transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * -4) * 0.01f, transform.position.z);
        }
        else
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive < 0) Destroy(gameObject);

            float distance = Vector3.Distance(transform.position, player.position);

            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            if (distance < 0.1f)
            {
                if (resource.name == "Time")
                {
                    GameManager.Instance.onAddTime(resource.Quantity);
                    Destroy(gameObject);
                }
                else
                if (carrier.AddCarriedResources(resource.ResourceType, resource.Quantity))
                    Destroy(gameObject);
                else Debug.Log("Not enough space");
            }
        }
    }
}
