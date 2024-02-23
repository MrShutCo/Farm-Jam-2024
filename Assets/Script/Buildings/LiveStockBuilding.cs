using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Script.Humans;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;



namespace Assets.Script.Buildings
{
    public class LiveStockBuilding : Building
    {
        internal class LiveStockSubsection
        {
            public List<Human> LiveStock;
            public List<Vector2> LiveStockPositions;

            public LiveStockSubsection(List<Vector2> liveStockPositions)
            {
                LiveStockPositions = liveStockPositions;
                LiveStock = new List<Human>(new Human[liveStockPositions.Count]);
            }
        }
        public int Level;

        [SerializeField] TextMeshProUGUI capacityText;

        List<LiveStockSubsection> workingHumans;

        public void Update()
        {
            //capacityText.text = $"{CurrHumans}/{MaxCapacity}";
        }
        public void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            foreach (var subsection in workingHumans)
            {
                for (int i = 0; i < subsection.LiveStockPositions.Count; i++)
                {
                    if (IsOver(subsection.LiveStockPositions[i], mouseWorldPosition) && subsection.LiveStock[i] == null)
                    {
                        subsection.LiveStock[i] = human;
                        human.transform.position = GetWorldPosition(subsection.LiveStockPositions[i]);
                    }
                }
            }
        }

        bool IsOver(Vector2 position, Vector2 mousePosition)
        {
            var a = new Rect(GetWorldPosition(position) - new Vector2(0.5f, 0.5f), new Vector2(1, 1));
            return a.Contains(mousePosition);
        }

        Vector2 GetWorldPosition(Vector2 localPostion)
        {
            return new Vector2(transform.position.x, transform.position.y) + localPostion;
        }
    }
}

