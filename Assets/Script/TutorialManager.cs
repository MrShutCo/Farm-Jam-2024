using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.UI;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public bool FirstTime;
    
    
    // Start is called before the first frame update
    void Start()
    {
        FirstTime = true;
    }

    private void OnEnable()
    {
        if (FirstTime)
        {
            DialogueManager.Instance.StartDialogue(new DialogueText("Welcome to hell!", null));
            FirstTime = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
