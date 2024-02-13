using Assets.Buildings;
using Assets.Script.Humans;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        public int CurrHumans;
        public int MaxCapacity;

        public Placeable PlaceableData;
        public Vector2Int Origin;

        public void CanAddNewConnectors(Building other)
        {
            foreach(var theirs in other.PlaceableData.Connectors)
            {
                var theirPosition = theirs.Position + other.Origin;
                foreach (var ours in PlaceableData.Connectors)
                {
                    var ourPosition = Origin + ours.Position;
                    // Left-Right connector
                    if (ourPosition.y == theirPosition.y && ourPosition.x + 1 == theirPosition.x
                        && theirs.EDirection == EDirection.Left && theirs.EDirection == EDirection.Right)
                    {
                        // Add connection
                    }
                }
            }
        }


        public bool AtCapacity() => CurrHumans == MaxCapacity;
        public bool CanBeWorked() => CurrHumans > 0;
    }
}