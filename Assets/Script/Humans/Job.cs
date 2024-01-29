using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Humans
{
    public abstract class Job
    {
        protected Rigidbody2D rb;
        public virtual void StartJob(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        public abstract void UpdateJob(Human human, double deltaTime);
        public abstract void FixedUpdateJob(Human human, double fixedDeltaTime);
        public abstract void StopJob();
        public Action OnStopJob { get; set; }
        public string Name { get; set; }
    }

    public class MoveToJob : Job
    {
        Vector3 target;
        int speed;
        List<Node2D> path;
        int currPathNodeDestination;

        public MoveToJob(Vector3 position)
        {
            target = position;
            Name = $"Move to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 3;
            Grid2D.onGridUpdated += onGridUpdated;
        }

        public override void StopJob()
        {
            Grid2D.onGridUpdated -= onGridUpdated;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
            if (path == null)
            {
                var pathfinding = human.GetComponent<Pathfinding2D>();
                path = pathfinding.FindPath(human.transform.position, target);
            }
            
        }
        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            if (path == null) return;
            var nodePos = path[currPathNodeDestination];
            var diffVector = nodePos.worldPosition - human.transform.position;
            diffVector.z = 0; // Account for different z levels
            if (diffVector.magnitude < 0.05)
            {
                if (currPathNodeDestination + 1 == path.Count)
                {
                    rb.velocity = Vector2.zero;
                    OnStopJob?.Invoke();
                } else
                {
                    currPathNodeDestination++;
                }
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }

        void onGridUpdated(int x, int y)
        {
            for (int i = currPathNodeDestination; i < path.Count; i++)
            {
                if (path[i].GridX == x && path[i].GridY == y)
                {
                    path = null;
                    break;
                }
            }
        }
    }

    public class WorkJob : Job
    {
        ResourceBuilding building;
        double timeWorkedOnJob;

        public WorkJob(ResourceBuilding building)
        {
            this.building = building;
            Name = $"Work {building.HarvestedResouce} at {building.transform.position}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
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
        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
        }
    }

    #region Outside Jobs
    public class Wander : Job
    {
        Vector3 target;
        float speed;
        float waitInterval = 1;
        float waitTimer = 0;
        // int tries;
        public Wander(Human human)
        {
            Debug.Log("Wander");
            //UpdateWanderTarget(human);
            Name = $"Wander to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 1;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            if (waitTimer <= 0)
            {
                var diffVector = target - human.transform.position;

                if (diffVector.magnitude < 0.05)
                {
                    rb.velocity = Vector2.zero;
                    UpdateWanderTarget(human);
                    waitTimer = waitInterval;
                }
                else
                {
                    rb.velocity = speed * diffVector.normalized;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                waitTimer -= (float)fixedDeltaTime;
            }
        }
        void UpdateWanderTarget(Human human)
        {
            target = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0) + human.transform.position;
            Debug.DrawRay(human.transform.position, target - human.transform.position, Color.red, 1);
        }
    }
    public class FleeTarget : Job
    {
        Vector3 target;
        float speed;
        public FleeTarget(Vector3 position)
        {
            target = position;
            Name = $"Flee to {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 2;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
            var diffVector = human.transform.position - target;

            if (diffVector.magnitude > 10)
            {
                rb.velocity = Vector2.zero;
                OnStopJob?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class ApproachTarget : Job
    {
        Vector3 target;
        float speed;
        float range;
        public ApproachTarget(Vector3 position)
        {
            target = position;
            Name = $"Approach {target}";
        }

        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            speed = 2;
            range = 5;
        }

        public override void StopJob()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateJob(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {

            var diffVector = target - human.transform.position;

            if (diffVector.magnitude < 10)
            {
                rb.velocity = Vector2.zero;
                //Enqueue wander
                OnStopJob?.Invoke();
            }
            else if (diffVector.magnitude < range)
            {
                //Enqueue attack
                OnStopJob?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class AttackTarget : Job
    {
        Vector3 target;
        float range;
        float attackInterval = 1;
        float attackTimer = 0;
        public override void StartJob(Rigidbody2D rb)
        {
            base.StartJob(rb);
            range = 5;
        }

        public override void StopJob()
        {

        }

        public override void UpdateJob(Human human, double deltaTime)
        {

            var diffVector = target - human.transform.position;
            if (diffVector.magnitude <= range && attackTimer <= 0)
            {
                Debug.Log("attack");
                attackTimer = attackInterval;
            }
            else if (diffVector.magnitude <= range && attackTimer > 0)
            {
                attackTimer -= (float)deltaTime;
            }
            else
            {
                //enqueue approach target
                OnStopJob?.Invoke();
            }
        }

        public override void FixedUpdateJob(Human human, double fixedDeltaTime)
        {
        }
    }
    #endregion
}