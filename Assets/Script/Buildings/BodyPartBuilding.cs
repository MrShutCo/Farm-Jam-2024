using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Script.Buildings
{
    /// <summary>
    /// Note: Working subsections in the prefab must be under a game object, preferably LevelN
    /// and must follow in the order of Flayee,Flayer,Flayer,Flayee,Flayer,Flayer... 
    /// </summary>
    public class BodyPartBuilding : ResourceBuilding
    {

        [SerializeField] private float secondPersonRateModifier;
        [SerializeField] private List<Transform> levelPositions;
        private List<SpriteRenderer> flayeeSpriteRenderers;

        private List<List<WorkingSubsection>> levelWorkingSubsections;
        [SerializeField] private SpriteRenderer background;
        
        public BodyPartBuilding()
        {
            _workTimer = new(1, true);
        }

        private void OnEnable()
        {
            levelWorkingSubsections = new List<List<WorkingSubsection>>();
            flayeeSpriteRenderers = new List<SpriteRenderer>();
            foreach (var positionSet in levelPositions)
            {
                var subsection = new List<WorkingSubsection>();
                var positions = positionSet.GetComponentsInChildren<Transform>()[1..];
                for (int i = 0; i < positions.Length/3; i++)
                {
                    var idx = 3 * i;
                    subsection.Add(new WorkingSubsection(positions[idx].localPosition,
                        new() { positions[idx+1].localPosition, positions[idx+2].localPosition }));
                    flayeeSpriteRenderers.Add(positions[idx].gameObject.GetComponent<SpriteRenderer>());
                }
                levelWorkingSubsections.Add(subsection);
            }
            // Set to level 0
            SetLevel(Level);
        }

        public void SetLevel(int level)
        {
            Level = level;
            background.sprite = buildingData.GetSprite(Level);
            _workingHumans = levelWorkingSubsections[Level];
        }

        public override void AssignHuman(Human human, Vector2 mouseWorldPosition)
        {
            int j = 0;
            int baseIndex = 0;
            if (Level == 1) baseIndex = 1;
            if (Level == 2) baseIndex = 3;
            foreach (var subsection in _workingHumans)
            {
                // assign to flayee
                if (IsOver(subsection.FlayeePosition, mouseWorldPosition) && subsection.Flayee == null)
                {
                    subsection.Flayee = human.GetComponent<HealthBase>();
                    human.transform.position = GetWorldPosition(subsection.FlayeePosition);
                    human.AddJob(new Job(human, "flay", new List<Task>(){new GetFlayed()},false));
                    human.Hide();
                    flayeeSpriteRenderers[j+baseIndex].sprite = buildingData.workedArea;
                    flayeeSpriteRenderers[j+baseIndex].color = Color.white;
                    subsection.SetFlayersAnim("IsScreaming", true);
                    if (subsection.IsBeingWorked())
                        callToAction.gameObject.SetActive(false);
                    
                    Debug.Log("Assigned flayee");
                }
                // assign to flayer
                for (int i = 0; i < subsection.FlayerPositions.Count; i++)
                {
                    if (IsOver(subsection.FlayerPositions[i], mouseWorldPosition) && subsection.Flayers[i] == null)
                    {
                        subsection.Flayers[i] = human;
                        human.transform.position = GetWorldPosition(subsection.FlayerPositions[i]);
                        human.GetComponent<Rigidbody2D>().isKinematic = true;
                        Debug.Log("Assigned flayer");
                        if (subsection.Flayee != null)
                        {
                            human.anim.SetBool("IsScreaming", true);
                        }
                    }
                }

                j++;
            }
        }

        public override bool TryUnassignHuman(Human human)
        {
            foreach (var subsection in _workingHumans)
            {
                for (int i = 0; i < subsection.Flayers.Count(); i++)
                {
                    if (subsection.Flayers[i] == human)
                    {
                        human.GetComponent<Rigidbody2D>().isKinematic = false;
                        subsection.Flayers[i] = null;
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Upgrade()
        {
            Level++;
            // Dear God, please never read this code, I beg of you
            if (Level == 1)
            {
                // TODO: this is a bad hack
                _packageSlots.Add(null);
                _packageSlots.Add(null);
                levelWorkingSubsections[1][0].Flayee = levelWorkingSubsections[0][0].Flayee;
                for (int i = 0; i < levelWorkingSubsections[1][0].Flayers.Count; i++)
                {
                    levelWorkingSubsections[1][0].Flayers[i] = levelWorkingSubsections[0][0].Flayers[i];
                }
                levelWorkingSubsections[0].Clear();
                var first = levelWorkingSubsections[1][0];
                if (first.Flayee)
                {
                    first.Flayee.transform.position = GetWorldPosition(first.FlayeePosition);
                    flayeeSpriteRenderers[0].color = Color.clear;
                    flayeeSpriteRenderers[1].color = Color.white;
                    flayeeSpriteRenderers[1].sprite = buildingData.workedArea;
                }

                for (var i = 0; i < first.FlayerPositions.Count; i++)
                {
                    if (first.Flayers[i] == null) continue;
                    first.Flayers[i].transform.position = GetWorldPosition(first.FlayerPositions[i]);
                }
            }
            if (Level == 2)
            {
                var currSub = levelWorkingSubsections[1];
                var newSub = levelWorkingSubsections[2];
                // Move over humans in code
                newSub[0].Flayee = currSub[0].Flayee;
                newSub[1].Flayee = currSub[1].Flayee;
                for (var i = 0; i < currSub.Count; i++)
                {
                    newSub[i].Flayers = currSub[i].Flayers;
                }
                levelWorkingSubsections[1].Clear();

                // For every subsection, update the positions for flayer and flayee
                for (var i = 0; i < newSub.Count; i++)
                {
                    if (newSub[i].Flayee)
                    {
                        newSub[i].Flayee.transform.position = GetWorldPosition(newSub[i].FlayeePosition);
                    }
                    // Update flayers position
                    for (var j = 0; j < newSub[i].FlayerPositions.Count; j++)
                    {
                        if (newSub[i].Flayers[j] == null) continue;
                        newSub[i].Flayers[j].transform.position = GetWorldPosition(newSub[i].FlayerPositions[j]);
                    }
                }
                // Update visuals. Clear old ones and possibly update new ones
                flayeeSpriteRenderers[1].color = Color.clear;
                flayeeSpriteRenderers[2].color = Color.clear;
                if (newSub[0].Flayee)
                {
                    flayeeSpriteRenderers[3].color = Color.white;
                    flayeeSpriteRenderers[3].sprite = buildingData.workedArea;
                }
                if (newSub[1].Flayee)
                {
                    flayeeSpriteRenderers[4].color = Color.white;
                    flayeeSpriteRenderers[4].sprite = buildingData.workedArea;
                }
            }
            
            background.sprite = buildingData.GetSprite(Level);
            _workingHumans = levelWorkingSubsections[Level];
        }

        protected override void OnWork()
        {
            float totalResourceGained = 0.0f;
            int i = 0;
            int baseIndex = 0;
            if (Level == 1) baseIndex = 1;
            if (Level == 2) baseIndex = 3;
            foreach (var group in _workingHumans)
            {
                if (group.Flayee is not null && group.IsBeingWorked() && !IsPackagesFull())
                {
                    totalResourceGained += group.Flayers.Sum(f => f == null ? 0 : f.GetWorkingRate(buildingData.resource));
                    if (group.Flayers.Count(f => f != null) > 1)
                    {
                        totalResourceGained *= secondPersonRateModifier;
                    }
                    callToAction.gameObject.SetActive(false); // Someone is working it, so clear any other flags
                    group.Flayee.TakeDamage(buildingData.damageToFlayee);
                    if (group.Flayee == null)
                    {
                        flayeeSpriteRenderers[i+baseIndex].color = Color.clear;
                        group.SetFlayersAnim("IsScreaming", false);
                        callToAction.gameObject.SetActive(true);
                    }
                    Console.WriteLine("Flayed damage taken");
                }

                i++;
            }

            _internalBuffer += totalResourceGained;
            if (_internalBuffer >= buildingData.internalBufferCapacity)
            {
                AddPacket();
                _internalBuffer -= buildingData.internalBufferCapacity;
            }

            if (_packageSlots.Count == buildingData.GetMaxPackages(Level))
            {
                callToAction.gameObject.SetActive(true);
            }

            internalBufferObject.UpdateStatusBar(_internalBuffer, (int)buildingData.internalBufferCapacity);
        }
    }
}