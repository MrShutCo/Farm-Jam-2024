using System.Collections;
using System.Collections.Generic;
using Assets.Script.Humans;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] float pickUpDistance = 2.5f;
    [SerializeField] float timeToLive = 10f;
    [SerializeField] EResource resourceType;

    void Start()
    {
        player = GameManager.Instance.Player;    
    }

    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive < 0) Destroy(gameObject);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > pickUpDistance) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        if (distance < 0.1f)
        {
            GameManager.Instance.AddResource(resourceType, 1);
            Destroy(gameObject);
        }
    }
}
