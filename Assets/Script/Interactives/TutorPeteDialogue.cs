using Assets.Script.UI;
using UnityEngine;

namespace Script.Interactives
{
    public class TutorPeteDialogue : MonoBehaviour
    {
        [SerializeField] DialogueText dialogue;
        private bool firstTimeTalking = true;

        private DialogueText firstTimeText;
        
        private void OnEnable()
        {
            dialogue = new DialogueText(" Do you have any questions for me?", new ()
            {
                new ("How do I bring back my husband?", 
                    new DialogueText("You must gather enough resources and sacrifice enough humans to light the six pillars and complete the sacrifice", null)),
                new ("How do I collect resources?", 
                    new DialogueText("First, there are two types of resources. Natural and human resources. Natural resources include Food, Wood, Steel, and Electronics. These will be used for building and upgrading machines." +
                                     "Human resources include Blood, Bones, and Organs. These are used in sacrifices, and will give you various upgrades to yourself. Human resources are extracted by flaying humans", null)), 
                new ("How does flaying work?", new DialogueText("Your husband can create flaying buildings that you can assign flayees and flayers to. You must have at least one of each in" +
                    " a building in order to generate resources. The flayee takes until dead, and the amount of resources harvested calculated by ", null)),
            });
            firstTimeText = new DialogueText("Tutor Pete: Howdy, I'm Tutor Pete. I'll be your guide through the underworld and help you raise your husband back to his former glory!", null, dialogue);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SetGameState(EGameState.Dialogue);
                DialogueManager.Instance.StartDialogue(firstTimeTalking ? firstTimeText : dialogue);
                firstTimeTalking = false;
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DialogueManager.Instance.EndDialogue(true);
            }
        }
    }
}