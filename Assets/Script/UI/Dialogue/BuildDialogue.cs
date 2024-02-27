namespace Assets.Script.UI
{
    public class BuildDialogue : DialogueText
    {
        public override void OnStart()
        {
            GameManager.Instance.SetGameState(EGameState.Build);
        }
    }
}