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

    public class Human : MonoBehaviour
    {
        public event Action<bool> OnWild;
        public Dictionary<EResource, int> Skills;
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Canvas Canvas;
        [SerializeField] bool wild;

        Transform _target;
        TextMeshProUGUI jobText;
        Rigidbody2D rb;

        List<FloatingStatusBar> skillBars;

        Queue<Job> currentJobs;

        public void Awake()
        {
            currentJobs = new Queue<Job>();
            jobText = GetComponentInChildren<TextMeshProUGUI>();
            skillBars = new List<FloatingStatusBar>();
            rb = GetComponent<Rigidbody2D>();
            var p = GetComponent<Pathfinding2D>();
            p.seeker = transform;
            p.GridOwner = GameObject.Find("Grid");
        }
        private void OnEnable()
        {
            rb.simulated = true;
        }

        private void OnDisable()
        {
            rb.simulated = false;
        }

        public void Start()
        {
            Skills = GameManager.Instance.InitializeResources();

            Canvas.gameObject.SetActive(false);
            float yOffset = 1f;

            foreach (var skill in Skills)
            {
                var status = Instantiate(StatusBar, Canvas.transform);
                var bar = status.GetComponent<FloatingStatusBar>();
                bar.UpdateStatusBar(skill.Value, 100);
                bar.UpdateText(skill.Key.ToString());
                status.transform.localPosition = new Vector3(4, yOffset, 0);
                skillBars.Add(bar);
                yOffset -= 0.5f;
            }

            OnWild?.Invoke(wild);

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
            Canvas.gameObject.SetActive(true);
            GameManager.Instance.CurrentlySelectedHuman?.Canvas.gameObject.SetActive(false);
            GameManager.Instance.CurrentlySelectedHuman = this;
        }

        public void ClearCurrentJobs()
        {
            for (int i = currentJobs.Count; i > 0; i--)
            {
                currentJobs.Dequeue().StopJob();
            }
        }
        public void OnMouseEnter()
        {
            Canvas.gameObject.SetActive(true);
        }

        public void OnMouseExit()
        {
            if (GameManager.Instance.CurrentlySelectedHuman == this) return;
            Canvas.gameObject.SetActive(false);
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
    }
}