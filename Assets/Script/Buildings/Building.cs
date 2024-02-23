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


        public bool AtCapacity() => CurrHumans == MaxCapacity;
        public bool CanBeWorked() => CurrHumans > 0;
    }
}