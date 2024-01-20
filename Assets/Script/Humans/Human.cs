using Assets.Script.Buildings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Script.Humans
{
 
    public enum EResource
    {
        Food, Wood, Iron, Electronics, Plutonium, 
    }

    public class Human : MonoBehaviour
    {
        public Dictionary<EResource, int> Skills;
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Canvas Canvas;
        TextMeshProUGUI jobText;

        List<FloatingStatusBar> skillBars;

        Queue<Job> currentJobs;

        public void Awake()
        {
            currentJobs = new Queue<Job>();
            jobText = GetComponentInChildren<TextMeshProUGUI>();
            skillBars = new List<FloatingStatusBar>();
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

        }

        public void AddJob(Job newJob)
        {
            if (currentJobs.Count == 0)
            {
                newJob.StartJob();
                newJob.OnStopJob += OnJobComplete;
            }
            currentJobs.Enqueue(newJob);
        }

        public void StopCurrentJob() => OnJobComplete();

        public void OnMouseEnter()
        {
            Canvas.gameObject.SetActive(true);
        }

        public void OnMouseExit()
        {
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
                currentJobs.Peek().UpdateJob(this, Time.deltaTime);
                jobText.text = currentJobs.Peek().Name;
            }

        }

        void OnJobComplete()
        {
            currentJobs.Peek().OnStopJob -= OnJobComplete;
            currentJobs.Peek().StopJob();
            currentJobs.Dequeue();
            jobText.text = "";
            if (currentJobs?.Count > 0)
            {
                currentJobs.Peek().StartJob();
                currentJobs.Peek().OnStopJob += OnJobComplete;
            }
        }
    }
}