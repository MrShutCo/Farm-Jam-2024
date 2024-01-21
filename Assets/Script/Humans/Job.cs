using Assets.Script.Buildings;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Humans
{
    public abstract class Job
    {
        public abstract void StartJob();
        public abstract void UpdateJob(Human human, double deltaTime);
        public abstract void StopJob();
        public Action OnStopJob { get; set; }
        public string Name { get; set; }
    }

    public class MoveToJob : Job
    {
        Vector3 target;
        int speed;

        public MoveToJob(Vector3 position)
        {
            target = position;
            Name = $"Move to {target}";
        }

        public override void StartJob()
        {
            speed = 2;
        }

        public override void StopJob() { }


        public override void UpdateJob(Human human, double deltaTime)
        {
            var diffVector = target - human.transform.position;
            diffVector.z = 0; // Account for different z levels
            if (diffVector.magnitude < 0.05)
            {
                human.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                OnStopJob?.Invoke();
            }
            else
            {
                human.GetComponent<Rigidbody2D>().velocity = speed * diffVector.normalized;
            }
        }
    }

    public class WorkJob : Job
    {
        Building building;
        double timeWorkedOnJob;

        public WorkJob(Building building)
        {
            this.building = building;
            Name = $"Work {building.HarvestedResouce} at {building.transform.position}";
        }

        public override void StartJob()
        {
            building.CurrHumans++;
        }

        public override void StopJob()
        {
            building.CurrHumans--;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            timeWorkedOnJob += deltaTime;

            if (timeWorkedOnJob >= building.TimeForOneSkillPoint)
            {
                human.LevelUpSkill(building.HarvestedResouce, 1);
                timeWorkedOnJob = 0;
            }
        }
    }
}