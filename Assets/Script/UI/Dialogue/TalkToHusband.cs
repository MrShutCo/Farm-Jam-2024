using System.Collections.Generic;

namespace Assets.Script.UI
{
    public class TalkToHusband : DialogueText
    {
        private List<string> _randomMessages = new()
        {
            "Cthylla! We need to cook Cthylla!",
            "Do you know what it's like to not have shit in over 5000 years?\nLet's just say, you won't want to be around when I'm resurrected...",
            "Do you ever feel like we're the bad guys?",
            "Cthylla, what is a PogChamp... \n\nwhy are the kids saying this to me... \nAm I out of touch?\nNo, it's the children who are wrong",
            "Patrolling hell almost makes you wish for a nuclear winter",
            "Man I could KILL for a fat burger right now!",
            "I've been thinking about joining a farming game jam, do you have any ideas?"
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