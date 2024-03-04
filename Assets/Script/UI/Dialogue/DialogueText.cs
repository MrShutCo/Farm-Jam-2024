using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Script.UI
{
    [Serializable]
    public class DialogueText
    {
        protected string _baseText;
        [CanBeNull] public List<DialogueOption> Options;
        [CanBeNull] public DialogueText NextText;
        
        public DialogueText(){}
        
        public DialogueText(string baseText, List<DialogueOption> options, DialogueText nextText = null)
        {
            _baseText = baseText;
            Options = options;
            NextText = nextText;
        }
        
        private List<KeyCode> numKeyCodes = new ()
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
        };

        public string GetText()
        {
            var s = _baseText;
            for (int i = 0; i < Options?.Count; i++)
                s += $"\n({i+1}): {Options[i].OptionText}";
            return s;
        }
        
        [CanBeNull]
        public DialogueText CheckButtonPressed()
        {
            if (Options is null) return null;
            for (var i = 0; i < Options.Count; i++)
            {
                if (Input.GetKeyDown(numKeyCodes[i]))
                {
                    if (Options[i].Predicate is null) return Options[i].SuccessfulNext;
                    return Options[i].Predicate() ? Options[i].SuccessfulNext : Options[i].FailedNext;
                }
            }
            return null;
        }
        
        public virtual void OnStart() {}
    }

    public class DialogueAction : DialogueText
    {
        private readonly Action _action;

        public DialogueAction(string text, Action action)
        {
            _action = action;
            _baseText = text;
        }

        public override void OnStart()
        {
            _action();
        }
    }
}