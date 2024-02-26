using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans.Traits;
using TMPro;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Assets.Script.Humans
{

    public enum EResource
    {
        Food, Wood, Steel, Electronics, Blood, Organs, Bones
    }

    public class Human : MonoBehaviour
    {
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Transform StatusPanel;
        [SerializeField] Transform targetSensor;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI traitText;
        
        HumanWildBehaviour wildBehaviour;
        WeaponSelector weaponSelector;

        Rigidbody2D rb;

        Pathfinding2D pathfinding;
        Queue<Job> currentJobs;
        public Queue<Job> CurrentJobs => currentJobs;
        public WeaponSelector WeaponSelector => weaponSelector;
        public HumanWildBehaviour WildBehaviour => wildBehaviour;

        Package holdingPackage;

        private EfficiencyProfile _efficiencyProfile;
        private List<Trait> _traits;

        public void Awake()
        {
            if (StatusPanel == null)
                StatusPanel = gameObject.transform.Find("StatusPanel");
            targetSensor = GetComponentInChildren<TargetSensor>().transform;
            nameText.text = Name;
            rb = GetComponent<Rigidbody2D>();
            wildBehaviour = GetComponent<HumanWildBehaviour>();
            weaponSelector = GetComponent<WeaponSelector>();
            pathfinding = GetComponent<Pathfinding2D>();
            pathfinding.seeker = transform;
            currentJobs = new Queue<Job>();
            _efficiencyProfile = new EfficiencyProfile();
            _traits = new List<Trait>()
            {
                new ResourceTrait(EResource.Blood, ERank.F),
                new ResourceTrait(EResource.Bones, ERank.F)
            };
            setTraitText();
        }

        public void InitializeHuman(string name, List<Trait> traits)
        {
            Name = name;
            _traits = traits;
            setTraitText();
            nameText.text = Name;
        }

        private void OnEnable()
        {
            StopAllJobs();
            rb.simulated = true;
            var world = GetComponentInParent<World>();
            pathfinding.GridOwner = world.Grid.gameObject;
            wildBehaviour.enabled = world.WorldType == EWorld.Wild;
        }

        private void OnDisable()
        {
            rb.simulated = false;
            GameManager.Instance.onHumanDie?.Invoke(this);
        }

        protected void Start()
        {
            SetUpStatusPanel();
        }

        void SetUpStatusPanel()
        {
            StatusPanel.gameObject.SetActive(false);
        }

        public bool CanBePickedUp() => currentJobs.Count == 0;

        public void HoldPackage(Package p)
        {
            holdingPackage = p;
            holdingPackage.transform.parent = transform;
            holdingPackage.transform.localPosition = new Vector3(0.5f, -0.11f,0);
        }

        public void DropoffPackage()
        {
            GameManager.Instance.AddResource(holdingPackage.Resource, holdingPackage.Amount);
            holdingPackage.Use();
        }

        public void SelectHuman()
        {
            StatusPanel.gameObject.SetActive(true);
            GameManager.Instance.CurrentlySelectedHuman?.Deselect();
            GameManager.Instance.CurrentlySelectedHuman = this;
        }

        public void Deselect()
        {
            StatusPanel.gameObject.SetActive(false);
        }
        
        #region Jobs

        public void StopCurrentJob()
        {
            if (currentJobs.Count == 0) return;
            currentJobs.Peek().StopJob();
        }

        public void StopAllJobs()
        {
            if (currentJobs.Count == 0) return;
            currentJobs.Peek().onJobComplete = null;
            while (currentJobs.Count > 0)
                currentJobs.Dequeue();
        }

        /// <summary>
        /// AddJob appends job to end of queue, and if there are no jobs, its starts
        /// </summary>
        public void AddJob(Job newJob)
        {
            currentJobs.Enqueue(newJob);
            if (currentJobs.Count == 1)
            {
                newJob.StartJob();
                newJob.onJobComplete += onJobComplete;
            }
        }

        void onJobComplete(Human h)
        {
            currentJobs.Peek().onJobComplete -= onJobComplete;
            currentJobs.Dequeue();
        }

        public void AddTaskToJob(Task task, bool stopCurrentTask)
        {
            if (currentJobs.Count == 0) return;
            currentJobs.Peek().AddTaskToJob(task, stopCurrentTask);
        }
        
        #endregion

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
            if (currentJobs != null && currentJobs.Count > 0)
            {
                currentJobs.Peek()?.Update(Time.deltaTime);
            }
        }

        void setTraitText()
        {
            string s = "";
            foreach (var trait in _traits)
            {
                s += trait + "\n";
            }

            traitText.text = s;
        }

        public void FixedUpdate()
        {
            if (currentJobs != null && currentJobs.Count > 0)
            {
                currentJobs.Peek()?.FixedUpdate(Time.deltaTime);
            }
            
            transform.localScale = new Vector3(rb.velocity.x > 0 ? 1 : -1, 1, 1);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Grid"))
            {
                pathfinding.GridOwner = other.transform.parent.gameObject;

            }
        }

        public float GetWorkingRate(EResource resource)
        {
            var newProfile = _traits.Aggregate(_efficiencyProfile, (current, trait) => trait.ActOn(current));
            return newProfile.PullRate[resource];
        }
    }
}