
using UnityEngine;

public class TriggerChangeGameState : MonoBehaviour
{
    [SerializeField] EGameState gameState;

    public void ChangeGameState()
    {
        GameManager.Instance.SetGameState(gameState);
    }
}
