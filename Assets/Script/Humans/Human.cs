using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Assets.Script.Humans
{

    public enum EResource
    {
        Wood, Steel, Electronics, Blood, Organs, Bones
    }

    public class Human : ControllerBase
    {
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Transform StatusPanel;
        [SerializeField] Transform targetSensor;

        HumanWildBehaviour wildBehaviour;

        Transform _target;
        TextMeshProUGUI jobText;
        Rigidbody2D rb;

        Task currentTask;

        public void Awake()
        {
            currentTask = null;
            if (StatusPanel == null)
                StatusPanel = gameObject.transform.Find("StatusPanel");
            targetSensor = GetComponentInChildren<TargetSensor>().transform;
            jobText = GetComponentInChildren<TextMeshProUGUI>();
            rb = GetComponent<Rigidbody2D>();
            wildBehaviour = GetComponent<HumanWildBehaviour>();
            var p = GetComponent<Pathfinding2D>();
            p.seeker = transform;
            //p.GridOwner = GameObject.Find("Grid");
        }
        private void OnEnable()
        {
            rb.simulated = true;
            //wildBehaviour.onTargetFound += OverrideJobs;
        }

        private void OnDisable()
        {
            rb.simulated = false;
            //wildBehaviour.onTargetFound -= OverrideJobs;
        }

        protected override void Start()
        {
            SetUpStatusPanel();
            base.Start();
        }

        void SetUpStatusPanel()
        {

            StatusPanel.gameObject.SetActive(false);
            /*float yOffset = 1f;
            foreach (var skill in Skills)
            {
                var status = Instantiate(StatusBar, StatusPanel.transform);
                var bar = status.GetComponent<FloatingStatusBar>();
                bar.UpdateStatusBar(skill.Value, 100);
                bar.UpdateText(skill.Key.ToString());
                status.transform.localPosition = new Vector3(4, yOffset, 0);
                skillBars.Add(bar);
                yOffset -= 0.5f;
            }*/
        }

        public bool IsIdle() => currentTask == null;

        public void SelectHuman()
        {
            StatusPanel.gameObject.SetActive(true);
            GameManager.Instance.CurrentlySelectedHuman?.StatusPanel.gameObject.SetActive(false);
            GameManager.Instance.CurrentlySelectedHuman = this;
        }

        public void ClearCurrentJobs()
        {
            // TODO
        }

        public void SetTask(Task task)
        {
            ClearCurrentJobs();
            currentTask = task;
            currentTask.StartTask(rb);
        }

        public void OnMouseEnter()
        {
            StatusPanel.gameObject.SetActive(true);
        }

        public void OnMouseExit()
        {
            if (GameManager.Instance.CurrentlySelectedHuman == this) return;
            StatusPanel.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (currentTask is null) return;
            jobText.text = currentTask.Name;
            currentTask.UpdateTask(this, Time.deltaTime);
        }

        public void FixedUpdate()
        {
            if (currentTask is null) return;
            jobText.text = currentTask.Name;
            currentTask.FixedUpdateTask(this, Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Grid"))
            {
                Debug.Log("Getting Grid");
                var p = GetComponent<Pathfinding2D>();
                p.GridOwner = other.gameObject;
            }
        }
        protected override void ChangeLocation(bool home)
        {
            ClearCurrentJobs();
            wildBehaviour.enabled = !home;
            targetSensor.gameObject.SetActive(!home);
            if (home)
            {
                var p = GetComponent<Pathfinding2D>();
                p.GridOwner = GameManager.Instance.PathfindingGrid.gameObject;
            }
            else
            {
                wildBehaviour.InitiateWildBehaviour();
            }
        }
    }
}