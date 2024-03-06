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
            dialogue = new DialogueText(" Do you have any questions for me?", new()
            {
                new ("How do I bring back my husband?",
                    new DialogueText("You must gather enough resources to light the six pillars and complete the sacrifice. But this task will not be easy. You will have to enslave my fellow people and slaugther them.", null)),
                new ("How do I collect resources?",
                    new DialogueText("There are two types of resources. Natural and human resources. Natural resources include Food, Wood, Steel, and Electronics. These will be used for building and upgrading machines", null,
                new DialogueText("Human resources include Blood, Bones, and Organs. These are used in sacrifices, and will give you various upgrades to yourself. Human resources are extracted by flaying us fleshy humans.", null))),
                new ("How does flaying work?", new DialogueText("Your husband can create flaying buildings that you can assign flayees and flayers to. You must have at least one of each in" +
                    " a building in order to generate resources. The flayee takes damage they're until dead, and the amount of resources harvested is calculated by number of people working them and their respective traits.", null,
                    new DialogueText("You also must assign haulers in the building below me to take away resources. Make sure you have enough haulers to prevent bottlenecks!", null))),
                new ("What are traits?", new DialogueText("Traits affect how good a person is at a particular skill. There's ranks F-S from worst to best.", null,
                    new DialogueText("*Extra Juicy - Bonus to blood flaying\n* Big Boned - Bonus to bone flaying\n* Gutsy - Bonus to organ harvesting", null))),
                new ("Why are you here?", new DialogueText("I fell asleep reading Lovecraftian Erotica and I woke up here...", null))
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