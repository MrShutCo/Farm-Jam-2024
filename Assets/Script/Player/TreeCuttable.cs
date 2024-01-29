using UnityEngine;
using System.Collections;

namespace Assets.Script.Player
{
	public class TreeCuttable : ToolHit
	{
        [SerializeField] float MaxHealth;
        float CurrentHealth;

        [SerializeField] GameObject pickUpDrop;
        [SerializeField] int dropCount = 5;
        [SerializeField] float spread = 3f;

        FloatingStatusBar healthBar;

        private void Awake()
        {
            healthBar = GetComponentInChildren<FloatingStatusBar>();
            CurrentHealth = MaxHealth;
        }

        public override void Hit(ToolsCharacterController tcc)
        {
            CurrentHealth -= 10;
            healthBar.UpdateStatusBar(CurrentHealth, MaxHealth);

            if (CurrentHealth < 0)
            {
                while (dropCount > 0)
                {
                    dropCount -= 1;
                    var dropPosition = transform.position;
                    dropPosition.x += spread * Random.value - spread / 2;
                    dropPosition.y += spread * Random.value - spread / 2;
                    GameObject go = Instantiate(pickUpDrop);
                    go.transform.position = dropPosition;
                }
                Destroy(gameObject);
            }
        }
    }
}