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
        Food, Wood, Iron, Electronics, Plutonium,
    }

    public class Human : ControllerBase
    {
        public Dictionary<EResource, int> Skills;
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Transform StatusPanel;
        [SerializeField] Transform targetSensor;

        HumanWildBehaviour wildBehaviour;

        Transform _target;
        TextMeshProUGUI jobText;
        Rigidbody2D rb;
        Pathfinding2D pathfinding;
        List<FloatingStatusBar> skillBars;
        Queue<Job> currentJobs;
        public Queue<Job> CurrentJobs => currentJobs;

        public void Awake()
        {
            currentJobs = new Queue<Job>();
            if (StatusPanel == null)
                StatusPanel = gameObject.transform.Find("StatusPanel");
            targetSensor = GetComponentInChildren<TargetSensor>().transform;
            jobText = GetComponentInChildren<TextMeshProUGUI>();
            skillBars = new List<FloatingStatusBar>();
            rb = GetComponent<Rigidbody2D>();
            wildBehaviour = GetComponent<HumanWildBehaviour>();
            pathfinding = GetComponent<Pathfinding2D>();
            pathfinding.seeker = transform;
        }
        private void OnEnable()
        {
            rb.simulated = true;
            wildBehaviour.onTargetFound += OverrideJobs;
        }

        private void OnDisable()
        {
            rb.simulated = false;
            wildBehaviour.onTargetFound -= OverrideJobs;
        }

        protected override void Start()
        {
            Skills = GameManager.Instance.InitializeResources();
            SetUpStatusPanel();
            base.Start();
        }

        void SetUpStatusPanel()
        {

            StatusPanel.gameObject.SetActive(false);
            float yOffset = 1f;
            foreach (var skill in Skills)
            {
                var status = Instantiate(StatusBar, StatusPanel.transform);
                var bar = status.GetComponent<FloatingStatusBar>();
                bar.UpdateStatusBar(skill.Value, 100);
                bar.UpdateText(skill.Key.ToString());
                status.transform.localPosition = new Vector3(4, yOffset, 0);
                skillBars.Add(bar);
                yOffset -= 0.5f;
            }
        }


        public void AddJob(Job newJob)
        {
            if (currentJobs.Count == 0)
            {
                newJob.StartJob(rb);
                newJob.OnStopJob += OnJobComplete;
            }
            currentJobs.Enqueue(newJob);
        }

        public void StopCurrentJob() => OnJobComplete();
        public bool IsIdle() => currentJobs.Count == 0;

        public void SelectHuman()
        {
            StatusPanel.gameObject.SetActive(true);
            GameManager.Instance.CurrentlySelectedHuman?.StatusPanel.gameObject.SetActive(false);
            GameManager.Instance.CurrentlySelectedHuman = this;
        }

        public void ClearCurrentJobs()
        {
            if (currentJobs.Count > 0)
                for (int i = 0; i < currentJobs.Count; i++)
                {
                    currentJobs.Dequeue().StopJob();
                }
        }

        public void OverrideJobs(Job job)
        {
            ClearCurrentJobs();
            AddJob(job);
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

        public void LevelUpSkill(EResource skill, int amount)
        {
            Skills[skill] += amount;
            for (int i = 0; i < Skills.Count; i++)
            {
                skillBars[i].UpdateStatusBar(Skills[(EResource)i], 100);
            }
        }

        public void Update()
        {
            if (currentJobs?.Count > 0)
            {
                jobText.text = currentJobs.Peek().Name;
                currentJobs.Peek().UpdateJob(this, Time.deltaTime);
            }

        }
        public void FixedUpdate()
        {
            if (currentJobs?.Count > 0)
            {
                jobText.text = currentJobs.Peek().Name;
                currentJobs.Peek().FixedUpdateJob(this, Time.fixedDeltaTime);

            }
        }

        void OnJobComplete()
        {
            if (currentJobs?.Count > 0)
            {
                currentJobs.Peek().OnStopJob -= OnJobComplete;
                currentJobs.Peek().StopJob();
                currentJobs.Dequeue();
            }
            jobText.text = "";
            if (currentJobs?.Count > 0)
            {
                currentJobs.Peek().StartJob(rb);
                currentJobs.Peek().OnStopJob += OnJobComplete;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Grid"))
            {
                Debug.Log("Getting Grid");
                pathfinding.GridOwner = other.gameObject;

            }
        }
        protected override void ChangeLocation(bool home)
        {
            ClearCurrentJobs();
            wildBehaviour.enabled = !home;
            targetSensor.gameObject.SetActive(!home);
            if (home)
            {
                pathfinding.GridOwner = GameManager.Instance.PathfindingGrid.gameObject;
            }
            else
            {
                pathfinding.GridOwner = GameManager.Instance.PathfindingGridOutside.gameObject;
                wildBehaviour.InitiateWildBehaviour();
            }
        }
    }
}