using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.UI
{

    public class DialogueOption
    {
        public string OptionText;
        public Func<bool> Predicate;
        public DialogueText SuccessfulNext;
        public DialogueText FailedNext;
        
        public DialogueOption(string optionText, DialogueText successfulNext)
        {
            OptionText = optionText;
            Predicate = () => true;
            SuccessfulNext = successfulNext;
        }
        
        public DialogueOption(string optionText, Func<bool> predicate, DialogueText successfulNext, DialogueText failedNext)
        {
            OptionText = optionText;
            Predicate = predicate;
            SuccessfulNext = successfulNext;
            FailedNext = failedNext;
        }
    }
    
    
}