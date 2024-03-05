using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Buildings;
using Assets.Script.Humans;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Buildings
{
    /// <summary>
    /// Note: Working subsections in the prefab must be under a game object, preferably LevelN
    /// and must follow in the order of Flayee,Flayer,Flayer,Flayee,Flayer,Flayer... 
    /// </summary>
    public class BodyPartBuilding : ResourceBuilding
    {

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
            _workingHumans = levelWorkingSubsections[Level];
        }

        public void SetLevel(int level)
        {
            Level = level;
            background.sprite = buildingData.GetSprite(Level);
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
                    subsection.SetFlayersAnim("ScreamTrigger");
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
                        human.GetComponent<CapsuleCollider2D>().enabled = false;
                        Debug.Log("Assigned flayer");
                        if (subsection.Flayee != null)
                        {
                            human.anim.SetTrigger("ScreamTrigger");
                        }
                    }
                }

                j++;
            }
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
                    callToAction.gameObject.SetActive(false); // Someone is working it, so clear any other flags
                    group.Flayee.TakeDamage(buildingData.damageToFlayee);
                    if (group.Flayee == null)
                    {
                        flayeeSpriteRenderers[i+baseIndex].color = Color.clear;
                        group.SetFlayersAnim("IdleTrigger");
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