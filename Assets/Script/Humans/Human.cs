using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Humans.Traits;
using TMPro;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace Assets.Script.Humans
{

    public enum EResource
    {
        Food, Wood, Steel, Electronics, Blood, Organs, Bones
    }

    public class Human : MonoBehaviour, ISpawnable
    {
        public string Name;

        [SerializeField] GameObject StatusBar;
        [SerializeField] Transform StatusPanel;
        [SerializeField] Transform targetSensor;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI traitText;
        [SerializeField] TextMeshProUGUI taskText;
        [SerializeField] Image[] traitImages;
        [SerializeField] TextMeshProUGUI[] traitValueTexts;

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
        private SpriteRenderer _spriteRenderer;
        public Animator anim { get; private set; }

        public float initSpeed;
        public float maxSpeed;
        public float initAttackRateMultiplier;
        public float currentAttackRateMultiplier;
        

        public void Awake()
        {
            if (StatusPanel == null)
                StatusPanel = gameObject.transform.Find("StatusPanel");

            targetSensor = GetComponentInChildren<TargetSensor>().transform;
            nameText.text = Name;
            maxSpeed = initSpeed;
            rb = GetComponent<Rigidbody2D>();
            wildBehaviour = GetComponent<HumanWildBehaviour>();
            weaponSelector = GetComponent<WeaponSelector>();
            pathfinding = GetComponent<Pathfinding2D>();
            pathfinding.seeker = transform;
            currentJobs = new Queue<Job>();
            _efficiencyProfile = new EfficiencyProfile();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            anim = _spriteRenderer.GetComponent<Animator>();
            _traits = new List<Trait>();
            //setTraitText();
            setTraitIcons();
#if UNITY_EDITOR
            taskText.enabled = true;
#else
            taskText.enabled = false;
#endif
        }

        public void InitializeHuman(string name, List<Trait> traits)
        {
            Name = name;
            _traits = traits;
            //setTraitText();
            setTraitIcons();
            nameText.text = Name;
            UpdateStats();
        }
        void UpdateStats()
        {
            HealthBase health = GetComponent<HealthBase>();
            foreach (var trait in _traits)
            {
                var addedHealth = health.InitHealth * trait.ActOn(_efficiencyProfile).HealthMultiplier;
                health.SetMaxHealth(health.MaxHealth + addedHealth);

                var addedSpeed = initSpeed * trait.ActOn(_efficiencyProfile).SpeedMultiplier;
                maxSpeed += addedSpeed;

                var addedAttackRate = initAttackRateMultiplier * trait.ActOn(_efficiencyProfile).AttackRateMultiplier;
                currentAttackRateMultiplier += addedAttackRate;
            }
        }


        private void OnEnable()
        {
            StopAllJobs();
            rb.simulated = true;
            EnableWorldBehaviour();
        }

        public void InitializeSpawnable()
        {
            EnableWorldBehaviour();
        }

        public void EnableWorldBehaviour()
        {
            var world = GetComponentInParent<World>();
            if (world == null)
            {
                Debug.LogWarning("Human must be a child of a world");
                return;
            }
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
            StatusPanel.gameObject.SetActive(false);
        }

        public bool CanBePickedUp()
        {
            return true;
        }

        public void HoldPackage(Package p)
        {
            holdingPackage = p;
            holdingPackage.transform.parent = transform;
            holdingPackage.transform.localPosition = new Vector3(0.5f, -0.11f, 0);
        }

        public void DropoffPackage()
        {
            GameManager.Instance.AddResource(holdingPackage.Resource, holdingPackage.Amount);
            holdingPackage.Use();
        }

        public void SelectHuman()
        {
            Select();
            anim.SetBool("IsScreaming", true);
            GameManager.Instance.CurrentlySelectedHuman?.Deselect();
            GameManager.Instance.CurrentlySelectedHuman = this;
            rb.velocity = Vector2.zero;
        }

        public void Deselect()
        {
            StatusPanel.gameObject.SetActive(false);
            anim.SetBool("IsScreaming", false);
        } 
        public void Select() => StatusPanel.gameObject.SetActive(true);


        public void Hide()
        {
            _spriteRenderer.enabled = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
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
            if (GameManager.Instance.GameState != EGameState.Normal) return;
            StatusPanel.gameObject.SetActive(true);
        }

        public void OnMouseExit()
        {
            if (GameManager.Instance.GameState != EGameState.Normal) return;
            StatusPanel.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (currentJobs != null && currentJobs.Count > 0)
            {
                taskText.text = currentJobs?.Peek()?.ActiveTaskText();
                currentJobs.Peek()?.Update(Time.deltaTime);
            }
            else
            {
                taskText.text = "Nothing";
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
        void setTraitIcons()
        {
            for (int i = 0; i < traitImages.Count(); i++)
            {
                if (i < _traits.Count())
                {
                    ResourceTrait resourceTrait = (ResourceTrait)_traits[i];
                    traitImages[i].enabled = true;
                    traitImages[i].sprite = GameManager.Instance.GetIcon(resourceTrait.GetResourceType());
                    traitValueTexts[i].text = resourceTrait.GetRank();
                }
                else
                {
                    traitImages[i].enabled = false;
                    traitValueTexts[i].text = "";
                }
            }
        }

        public void FixedUpdate()
        {
            if (currentJobs != null && currentJobs.Count > 0)
            {
                currentJobs.Peek()?.FixedUpdate(Time.deltaTime);
            }

            _spriteRenderer.transform.localScale = new Vector3(rb.velocity.x > 0 ? 1 : -1, 1, 1);
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
            return newProfile.WorkRate[resource];
        }
    }
}