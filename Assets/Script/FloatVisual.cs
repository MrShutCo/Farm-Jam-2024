using System;
using UnityEngine;

    public class FloatVisual : MonoBehaviour
    {
        [Header("Float Params")] 
        [SerializeField] private float maxShift;
        [SerializeField] private float floatPeriod;

        private void Update()
        {
            var p = transform.localPosition;
            p.y = (float)(maxShift * Math.Sin(floatPeriod * Time.time));
            transform.localPosition = p;
        }
    }