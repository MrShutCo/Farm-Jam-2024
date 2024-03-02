using System;
using UnityEngine;

    public class FloatVisual : MonoBehaviour
    {
        [Header("Float Params")] 
        [SerializeField] private float maxShift;
        [SerializeField] private float floatPeriod;

        private Vector3 _initialPosition;
        
        private void OnEnable()
        {
            _initialPosition = transform.localPosition;
        }

        private void Update()
        {
            var p = transform.localPosition;
            p.y = _initialPosition.y + (float)(maxShift * Math.Sin(floatPeriod * Time.time));
            transform.localPosition = p;
        }
    }