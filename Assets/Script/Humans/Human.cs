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

    public class Human : ControllerBase
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

        Task currentTask;
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
            currentTask = null;
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

        private void OnEnable()
        {
            rb.simulated = true;
            //wildBehaviour.onTargetFound += OverrideJobs;
        }

        private void OnDisable()
        {
            rb.simulated = false;
            GameManager.Instance.onHumanDie?.Invoke(this);
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

        public bool CanBePickedUp() => currentTask == null && currentJobs.Count == 0;
        
        public bool IsIdle() => currentTask == null;

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

        public void ClearCurrentJobs()
        {
            // TODO
        }

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
            if (currentJobs != null && currentJobs.Count > 0)
            {
                currentJobs.Peek()?.Update(Time.deltaTime);
            }

            if (currentTask is null) return;
            
            currentTask.UpdateTask(this, Time.deltaTime);
            

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
            if (currentTask is null) return;
            nameText.text = currentTask.Name;
            currentTask.FixedUpdateTask(this, Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Grid"))
            {
                pathfinding.GridOwner = other.transform.parent.gameObject;

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

        public float GetWorkingRate(EResource resource)
        {
            var newProfile = _traits.Aggregate(_efficiencyProfile, (current, trait) => trait.ActOn(current));
            return newProfile.PullRate[resource];
        }
    }
}