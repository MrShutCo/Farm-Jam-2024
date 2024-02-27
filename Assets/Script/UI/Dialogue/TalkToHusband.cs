using System.Collections.Generic;

namespace Assets.Script.UI
{
    public class TalkToHusband : DialogueText
    {
        private List<string> _randomMessages = new ()
        {
            "Cynthia! We need to cook Cynthia!",
            "I haven't shit in 500 years",
            "Do you ever feel like we're the bad guys?",
            "Cynthia, what is a PogChamp... \n\nwhy are the kids saying this to me... \n\t\tAm I out of touch?\nNo, it is the kids who are wrong",
            "Patrolling hell almost makes you wish for a nuclear winter",
        };

        public TalkToHusband()
        {
            _baseText = _randomMessages[UnityEngine.Random.Range(0, _randomMessages.Count)];
        }

        public override void OnStart()
        {
            _baseText = _randomMessages[UnityEngine.Random.Range(0, _randomMessages.Count)];
            base.OnStart(); 
        }
    }
}