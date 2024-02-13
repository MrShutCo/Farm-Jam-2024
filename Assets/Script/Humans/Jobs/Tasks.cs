using Assets.Script.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Humans
{
    public abstract class Task
    {
        protected Rigidbody2D rb;
        public virtual void StartTask(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        public abstract void UpdateTask(Human human, double deltaTime);
        public abstract void FixedUpdateTask(Human human, double fixedDeltaTime);
        public abstract void StopTask();
        public Action OnStopTask { get; set; }
        public string Name { get; set; }
    }

    public class MoveToTask : Task
    {
        Vector3 target;
        int speed;
        List<Node2D> path;
        int currPathNodeDestination;

        public MoveToTask(Vector3 position)
        {
            target = position;
            Name = $"Move to {target}";
        }

        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);
            speed = 3;
            Grid2D.onGridUpdated += onGridUpdated;
        }

        public override void StopTask()
        {
            Grid2D.onGridUpdated -= onGridUpdated;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            if (path == null)
            {
                var pathfinding = human.GetComponent<Pathfinding2D>();
                path = pathfinding.FindPath(human.transform.position, target);
            }

        }
        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            if (path == null || path.Count == 0) return;
            var nodePos = path[currPathNodeDestination];
            var diffVector = nodePos.worldPosition - human.transform.position;
            diffVector.z = 0; // Account for different z levels
            if (diffVector.magnitude < 0.05)
            {
                if (currPathNodeDestination + 1 == path.Count)
                {
                    rb.velocity = Vector2.zero;
                    currPathNodeDestination = 0;
                    path = null;
                    OnStopTask?.Invoke();

                }
                else
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

    public class WorkTask : Task
    {
        ResourceBuilding building;
        double timeWorkedOnTask;

        public WorkTask(ResourceBuilding building)
        {
            this.building = building;
            Name = $"Work {building.HarvestedResouce} at {building.transform.position}";
        }

        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);

        }

        public override void StopTask()
        {
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            timeWorkedOnTask += deltaTime;

            if (timeWorkedOnTask >= building.TimeToCollect)
            {
                OnStopTask?.Invoke();
                timeWorkedOnTask = 0;
            }
        }
        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
        }
    }

    public class DropoffResources : Task
    {
        EResource _resource;
        int _amount;

        float timeSpentDroppingOff;
        float timeToDropOff = 1f;

        public DropoffResources(EResource resource, int amount)
        {
            _resource = resource;
            _amount = amount;
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
        }

        public override void StopTask()
        {
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
            
            timeSpentDroppingOff += (float)deltaTime;

            if (timeSpentDroppingOff > timeToDropOff)
            {
                GameManager.Instance.AddResource(_resource, _amount);
                timeSpentDroppingOff = 0;
                OnStopTask?.Invoke();
            }
        }
    }

    public class GetFlayed : Task
    {
        float timeSpentBeingFlayed;
        float timeUntilDamageTaken = 6f;

        public GetFlayed()
        {
            Name = "Getting Freaking Flayed :(";
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            timeSpentBeingFlayed += (float)fixedDeltaTime;
            if (timeSpentBeingFlayed > timeUntilDamageTaken)
            {
                if (human.TryGetComponent(out HealthBase health))
                {
                    health.TakeDamage(1);
                    timeSpentBeingFlayed = 0;
                }
            }
        }

        public override void StopTask()
        {
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }
    }

    #region Outside Tasks
    public class Wander : Task
    {
        Vector3 target;
        float wanderDistance = 10;
        float timeOfLastTarget;
        float speed;
        float waitInterval = 1;
        float waitTimer = 0;
        // int tries;
        public Wander(Human human)
        {
            Debug.Log("Wander");
            Name = $"Wander to {target}";
        }

        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);
            speed = 1;
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
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
            if (Time.time - timeOfLastTarget < 5) return;
            timeOfLastTarget = Time.time;
            target = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-wanderDistance, wanderDistance), 0) + human.transform.position;
            Debug.DrawRay(human.transform.position, target - human.transform.position, Color.red, 1);

        }
    }
    public class FleeTarget : Task
    {
        Vector3 position;
        float speed;
        public FleeTarget(Transform target)
        {
            Debug.Log("Flee");
            position = target.position;
            Name = $"Flee to {target}";
        }

        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);
            speed = 2;
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
            var diffVector = human.transform.position - position;

            if (diffVector.magnitude > 10)
            {
                rb.velocity = Vector2.zero;
                OnStopTask?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class ApproachTarget : Task
    {
        Vector3 position;
        float speed;
        float range;
        public ApproachTarget(Transform target)
        {
            Debug.Log("Approach");
            position = target.position;
            Name = $"Approach {target}";
        }

        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);
            speed = 2;
            range = 5;
        }

        public override void StopTask()
        {
            rb.velocity = Vector2.zero;
        }

        public override void UpdateTask(Human human, double deltaTime)
        {
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {

            var diffVector = position - human.transform.position;

            if (diffVector.magnitude > 10)
            {
                rb.velocity = Vector2.zero;
                //Enqueue wander
                OnStopTask?.Invoke();
            }
            else if (diffVector.magnitude < range)
            {
                //Enqueue attack
                OnStopTask?.Invoke();
            }
            else
            {
                rb.velocity = speed * diffVector.normalized;
            }
        }
    }
    public class AttackTarget : Task
    {
        Vector3 target;
        float range;
        float attackInterval = 1;
        float attackTimer = 0;
        public override void StartTask(Rigidbody2D rb)
        {
            base.StartTask(rb);
            range = 5;
        }

        public override void StopTask()
        {

        }

        public override void UpdateTask(Human human, double deltaTime)
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
                OnStopTask?.Invoke();
            }
        }

        public override void FixedUpdateTask(Human human, double fixedDeltaTime)
        {
        }
    }
    #endregion
}